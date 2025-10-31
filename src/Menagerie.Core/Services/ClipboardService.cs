using Menagerie.Core.Models.Configuration;
using Menagerie.Core.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace Menagerie.Core.Services;

public class ClipboardService : IClipboardService, IAsyncDisposable
{
    #region Members

    private readonly ILogger<ClipboardService> _logger;
    private readonly IAppConfigurationService _appConfigurationService;
    private readonly IGameService _gameService;
    private readonly ITextParsingService _textParsingService;

    private readonly SemaphoreSlim _clipboardLock = new(1, 1);
    private string? _previousValue;
    private Thread? _pollingThread;
    private readonly CancellationTokenSource _pollingThreadCts = new();

    #endregion

    #region Constructors

    public ClipboardService(
        ILogger<ClipboardService> logger,
        IAppConfigurationService appConfigurationService,
        IGameService gameService,
        ITextParsingService textParsingService
    )
    {
        _logger = logger;
        _appConfigurationService = appConfigurationService;
        _gameService = gameService;
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
        Initialize();
    }

    #endregion

    #region Public methods

    public Task<string?> GetTextAsync()
    {
        return GetClipboardValueAsync();
    }

    public Task<bool> SetTextAsync(string text)
    {
        return SetClipboardValueAsync(text);
    }

    public Task<bool> ResetTextAsync()
    {
        return ResetClipboardValueAsync();
    }

    #endregion

    #region Private methods

    private async Task PollClipboard()
    {
        Queue<string> exclusionQueue = new();
        var lastValue = await GetClipboardValueAsync();

        while (!_pollingThreadCts.IsCancellationRequested)
        {
            var appConfiguration = await _appConfigurationService.GetConfigurationAsync();

            var value = await GetClipboardValueAsync();
            if (!string.IsNullOrEmpty(value))
            {
                if (!exclusionQueue.Contains(value))
                {
                    exclusionQueue.Enqueue(value);

                    if (value != lastValue)
                    {
                        _textParsingService.ParseOutgoingTrade(value);
                    }

                    while (exclusionQueue.Count > appConfiguration.Clipboard.ExclusionQueueLength)
                    {
                        exclusionQueue.Dequeue();
                    }
                }

                lastValue = value;
            }

            await Task.Delay(appConfiguration.Clipboard.PollingRate);
        }
    }

    private async Task<string?> GetClipboardValueAsync(bool needLock = true)
    {
        if (needLock)
        {
            await _clipboardLock.WaitAsync();
        }

        try
        {
            var appConfiguration = await _appConfigurationService.GetConfigurationAsync();
            await Task.Delay(appConfiguration.Clipboard.InternalDelay);
            return await TextCopy.ClipboardService.GetTextAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting clipboard value");
        }
        finally
        {
            if (needLock)
            {
                _clipboardLock.Release();
            }
        }

        return null;
    }

    private async Task<bool> SetClipboardValueAsync(string value, bool needLock = true)
    {
        _previousValue = await GetClipboardValueAsync();

        if (needLock)
        {
            await _clipboardLock.WaitAsync();
        }

        try
        {
            var appConfiguration = await _appConfigurationService.GetConfigurationAsync();
            for (var i = 0; i < appConfiguration.Clipboard.NbRetriesOnFail; ++i)
            {
                await TextCopy.ClipboardService.SetTextAsync(value);
                await Task.Delay(appConfiguration.Clipboard.InternalDelay);
                if (await GetClipboardValueAsync() == value) return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while setting clipboard value");
        }
        finally
        {
            if (needLock)
            {
                _clipboardLock.Release();
            }
        }

        return false;
    }

    private async Task<bool> ResetClipboardValueAsync()
    {
        if (string.IsNullOrEmpty(_previousValue)) return false;

        await _clipboardLock.WaitAsync();

        try
        {
            await SetClipboardValueAsync(_previousValue, false);
            _previousValue = null;
            return true;
        }
        finally
        {
            _clipboardLock.Release();
        }
    }

    private void Initialize()
    {
        _pollingThread = new Thread(() => PollClipboard().Wait())
        {
            IsBackground = true
        };
        _pollingThread.Start();
    }

    private async Task StopPollingThread()
    {
        await _pollingThreadCts.CancelAsync();
        var appConfiguration = await _appConfigurationService.GetConfigurationAsync();
        await Task.Delay(2 * appConfiguration.Clipboard.PollingRate);
    }

    #endregion
}