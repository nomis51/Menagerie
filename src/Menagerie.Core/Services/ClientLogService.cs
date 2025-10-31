using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class ClientLogService : IClientLogService, IAsyncDisposable
{
    #region Members

    private readonly IGameService _gameService;
    private readonly IAppConfigurationService _appConfigurationService;
    private readonly ITextParsingService _textParsingService;

    private Thread? _pollingThread;
    private readonly CancellationTokenSource _pollingThreadCts = new();
    private string? _clientLogFilePath;
    private long _eofPosition;

    #endregion

    #region Constructors

    public ClientLogService(
        IGameService gameService,
        IAppConfigurationService appConfigurationService,
        ITextParsingService textParsingService
    )
    {
        _gameService = gameService;
        _appConfigurationService = appConfigurationService;
        _textParsingService = textParsingService;
        _gameService.GameProcessChanged += GameProcessChanged;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        _gameService.GameProcessChanged -= GameProcessChanged;
        await StopPollingThread();
    }

    #endregion

    #region Events

    private void GameProcessChanged(object? sender, int e)
    {
        StopPollingThread().Wait();
        Initialize().Wait();
    }

    #endregion

    #region Private methods

    private async Task PollClientLogs()
    {
        while (!_pollingThreadCts.IsCancellationRequested)
        {
            var appConfiguration = await _appConfigurationService.GetConfigurationAsync();

            var eofPosition = await GetEofPosition();
            if (eofPosition != _eofPosition)
            {
                foreach (var line in File.ReadLines(_clientLogFilePath!))
                {
                    _textParsingService.ParseIncomingTrade(line);
                }
            }

            _eofPosition = eofPosition;

            await Task.Delay(appConfiguration.ClientLog.PollingRate);
        }
    }

    private async Task StopPollingThread()
    {
        await _pollingThreadCts.CancelAsync();
        var appConfiguration = await _appConfigurationService.GetConfigurationAsync();
        await Task.Delay(2 * appConfiguration.ClientLog.PollingRate);
    }

    private async Task Initialize()
    {
        _clientLogFilePath = _gameService.GetGameClientLogFilePathAsync();
        if (_clientLogFilePath is null) return;

        _eofPosition = await GetEofPosition();

        _pollingThread = new Thread(() => PollClientLogs().Wait())
        {
            IsBackground = true
        };
        _pollingThread.Start();
    }

    private async Task<long> GetEofPosition()
    {
        if (_clientLogFilePath is null) return 0;

        await using var file = File.Open(_clientLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return file.Length - 1;
    }

    #endregion
}