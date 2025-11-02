using System.IO.Abstractions;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class ClientLogService : IClientLogService, IAsyncDisposable
{
    #region Members

    private readonly IGameService _gameService;
    private readonly IAppConfigurationService _appConfigurationService;
    private readonly ITextParsingService _textParsingService;
    private readonly IFileSystem _fileSystem;

    private Thread? _pollingThread;
    private CancellationTokenSource? _pollingThreadCts = null;
    private string? _clientLogFilePath;
    private long _eofPosition;

    #endregion

    #region Constructors

    public ClientLogService(
        IGameService gameService,
        IAppConfigurationService appConfigurationService,
        ITextParsingService textParsingService,
        IFileSystem fileSystem
    )
    {
        _gameService = gameService;
        _appConfigurationService = appConfigurationService;
        _textParsingService = textParsingService;
        _fileSystem = fileSystem;
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

    private void GameProcessChanged(object? sender, EventArgs e)
    {
        Task.Run(async () =>
        {
            await StopPollingThread();
            await Initialize();
        });
    }

    #endregion

    #region Private methods

    private async Task PollClientLogs()
    {
        while (!_pollingThreadCts?.IsCancellationRequested ?? false)
        {
            var appConfiguration = await _appConfigurationService.GetConfigurationAsync();

            var currentPosition = await GetEofPosition();
            if (currentPosition != _eofPosition)
            {
                await using var fs = _fileSystem.File.Open(
                    _clientLogFilePath!,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite
                );
                if (fs.CanSeek)
                {
                    fs.Position = _eofPosition;
                    using var reader = new StreamReader(fs);

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(line)) continue;

                        _textParsingService.ParseIncomingTrade(line);
                    }
                }

                _eofPosition = currentPosition;
            }

            await Task.Delay(appConfiguration.ClientLog.PollingRate);
        }
    }

    private async Task StopPollingThread()
    {
        if (_pollingThreadCts is not null)
        {
            await _pollingThreadCts.CancelAsync();
        }

        var appConfiguration = await _appConfigurationService.GetConfigurationAsync();
        await Task.Delay(appConfiguration.ClientLog.PollingRate);

        while (_pollingThread is not null && _pollingThread.IsAlive)
        {
            await Task.Delay(50);
        }

        _pollingThread = null;
        _pollingThreadCts = null;
    }

    private async Task Initialize()
    {
        _clientLogFilePath = _gameService.GetGameClientLogFilePathAsync();
        if (_clientLogFilePath is null) return;

        _eofPosition = await GetEofPosition();

        _pollingThreadCts = new CancellationTokenSource();
        _pollingThread = new Thread(() => PollClientLogs().Wait())
        {
            IsBackground = true
        };
        _pollingThread.Start();
    }

    private async Task<long> GetEofPosition()
    {
        if (_clientLogFilePath is null) return 0;

        await using var file = _fileSystem.File
            .Open(_clientLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return file.Length - 1;
    }

    #endregion
}