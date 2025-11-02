using Menagerie.Core.Windows.Helpers.Abstractions;
using Microsoft.Extensions.Logging;
using TextCopy;

namespace Menagerie.Core.Windows.Helpers;

public class ClipboardHelper : IClipboardHelper
{
    #region Members

    private readonly ILogger<ClipboardHelper> _logger;
    private readonly Clipboard _clipboard = new();
    private readonly SemaphoreSlim _clipboardLock = new(1, 1);
    private string? _previousValue;

    #endregion

    #region Constructors

    public ClipboardHelper(ILogger<ClipboardHelper> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Public methods

    public Task<bool> SetClipboardTextAsync(string text)
    {
        return SetClipboardTextAsync(text, true);
    }

    public Task<string?> GetClipboardTextAsync()
    {
        return GetClipboardTextAsync(true);
    }

    public async Task<bool> ResetClipboardTextAsync()
    {
        if (string.IsNullOrEmpty(_previousValue)) return false;

        await _clipboardLock.WaitAsync();

        try
        {
            await SetClipboardTextAsync(_previousValue, false);
            _previousValue = null;
            return true;
        }
        finally
        {
            _clipboardLock.Release();
        }
    }

    #endregion

    #region Private methods

    private async Task<string?> GetClipboardTextAsync(bool needLock)
    {
        if (needLock)
        {
            await _clipboardLock.WaitAsync();
        }

        try
        {
            await Task.Delay(50);
            return await _clipboard.GetTextAsync();
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

    private async Task<bool> SetClipboardTextAsync(string text, bool needLock)
    {
        _previousValue = await GetClipboardTextAsync(needLock);

        if (needLock)
        {
            await _clipboardLock.WaitAsync();
        }

        try
        {
            await _clipboard.SetTextAsync(text);
            await Task.Delay(50);

            var nbRetries = 3;
            while (nbRetries-- > 0)
            {
                if (await GetClipboardTextAsync(needLock) == text) return true;
            }

            return false;
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

    #endregion
}