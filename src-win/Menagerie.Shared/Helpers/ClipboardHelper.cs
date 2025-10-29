using Serilog;
using TextCopy;

namespace Menagerie.Shared.Helpers;

public static class ClipboardHelper
{
    #region Constants

    private static readonly object ClipboardLock = new();
    private const int NbTry = 3;

    #endregion

    #region Members

    private static string _previousValue = string.Empty;

    #endregion

    #region Public methods

    public static void SetClipboard(string value, int delay = 50)
    {
        lock (ClipboardLock)
        {
            _previousValue = GetClipboardValue();

            try
            {
                for (var i = 0; i < NbTry; ++i)
                {
                    ClipboardService.SetText(value);
                    Thread.Sleep(10);
                    if (GetClipboardValue() == value) break;
                }

                Thread.Sleep(delay);
            }
            catch (Exception e)
            {
                Log.Error("Unable to set clipboard {message}", e.Message);
            }
        }
    }

    public static string GetClipboardValue(int delay = 0)
    {
        try
        {
            if (delay is > 0) Thread.Sleep(delay);
            return ClipboardService.GetText() ?? string.Empty;
        }
        catch (Exception e)
        {
            Log.Error("Unable to get clipboard value {message}", e.Message);
        }

        return string.Empty;
    }

    public static void ResetClipboardValue(int delay = 50)
    {
        if (string.IsNullOrEmpty(_previousValue)) return;

        Task.Run(() =>
        {
            lock (ClipboardLock)
            {
                Thread.Sleep(delay);

                try
                {
                    ClipboardService.SetText(_previousValue);
                }
                catch (Exception e)
                {
                    Log.Error("Unable to reset clipboard value {message}", e.Message);
                }

                _previousValue = string.Empty;
            }
        });
    }

    #endregion
}