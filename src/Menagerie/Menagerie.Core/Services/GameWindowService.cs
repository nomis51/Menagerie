using System.Diagnostics;
using Menagerie.Core.Services.Abstractions;
using Menagerie.Data.WinApi;
using Serilog;

namespace Menagerie.Core.Services;

public class GameWindowService : IGameWindowService
{
    #region Constants

    private const int OverlayHideDelay = 3000;
    private static readonly object ShowHideLock = new();

    #endregion

    #region Members

    private int _processId;
    private IntPtr _overlayHandle;
    private bool _isOverlayVisible = true;

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        AutoHideOverlay();
        return Task.CompletedTask;
    }

    public void SetProcessId(int processId)
    {
        _processId = processId;
    }

    public bool FocusGameWindow()
    {
        if (_processId == 0) return false;

        try
        {
            var process = Process.GetProcessById(_processId);
            if (process is null || process.HasExited) return false;

            if (IsGameWindowFocused()) return true;

            var noTry = 0;
            while (!(User32.ShowWindow(process.MainWindowHandle, 5) &&
                     User32.SetForegroundWindow(process.MainWindowHandle) &&
                     IsGameWindowFocused()) && noTry < 3)
            {
                ++noTry;
            }

            return noTry < 3;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IsGameWindowFocused(int delay = 50)
    {
        var process = Process.GetProcessById(_processId);
        if (process is null || process.HasExited) return false;

        Thread.Sleep(delay);
        var current = User32.GetForegroundWindow();
        return current == process.MainWindowHandle;
    }

    public void AutoHideOverlay()
    {
        new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    try
                    {
                        if (IsGameWindowFocused())
                        {
                            AppService.Instance.ShowOverlay();
                        }
                        else
                        {
                            AppService.Instance.HideOverlay();
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warning("Unable to show/hide overlay: {Message} {Callstack}", e.Message, e.StackTrace);
                    }
                }
            })
            {
                IsBackground = true
            }
            .Start();
    }

    #endregion
}