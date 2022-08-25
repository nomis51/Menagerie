using System.Diagnostics;
using Menagerie.Data.WinApi;
using Menagerie.Shared.Abstractions;

namespace Menagerie.Data.Services;

public class GameWindowService : IService
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

    public void SetOverlayHandle(IntPtr handle)
    {
        _overlayHandle = handle;

        User32.SetWindowLong(
            _overlayHandle,
            User32.GWL_EX_STYLE,
            (User32.GetWindowLong(_overlayHandle, User32.GWL_EX_STYLE) | User32.WS_EX_TOOLWINDOW) & ~User32.WS_EX_APPWINDOW
        );
    }

    public void ToggleOverlay()
    {
        if (_isOverlayVisible)
        {
            HideOverlay();
        }
        else
        {
            ShowOverlay();
        }
    }

    public bool FocusOverlay()
    {
        if (_overlayHandle == IntPtr.Zero) return false;

        User32.SetForegroundWindow(_overlayHandle);
        return true;
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

    public bool IsOverlayFocused(int delay = 50)
    {
        Thread.Sleep(delay);
        var current = User32.GetForegroundWindow();
        return current == _overlayHandle;
    }

    public bool IsGameWindowFocused(int delay = 50)
    {
        var process = Process.GetProcessById(_processId);
        if (process is null || process.HasExited) return false;

        Thread.Sleep(delay);
        var current = User32.GetForegroundWindow();
        return current == process.MainWindowHandle;
    }

    #endregion

    #region Private methods

    private void AutoHideOverlay()
    {
        new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(500);

                    if (IsGameWindowFocused() || IsOverlayFocused())
                    {
                        ShowOverlay();
                    }
                    else
                    {
                        HideOverlay();
                    }
                }
            })
            {
                IsBackground = true
            }
            .Start();
    }

    private void ShowOverlay()
    {
        if (_isOverlayVisible) return;

        lock (ShowHideLock)
        {
            AppDataService.Instance.ShowOverlay();
            _isOverlayVisible = true;
        }
    }

    private void HideOverlay()
    {
        if (!_isOverlayVisible) return;

        lock (ShowHideLock)
        {
            AppDataService.Instance.HideOverlay();
            _isOverlayVisible = false;
        }
    }

    #endregion
}