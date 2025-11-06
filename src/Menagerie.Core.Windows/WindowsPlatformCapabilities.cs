using System.Diagnostics;
using Desktop.Robot;
using Menagerie.Core.Shared.Abstractions;
using Menagerie.Core.Windows.Helpers.Abstractions;
using Menagerie.Core.Windows.Win32;
using Microsoft.Extensions.Logging;

namespace Menagerie.Core.Windows;

public class WindowsPlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly ILogger<WindowsPlatformCapabilities> _logger;
    private readonly IClipboardHelper _clipboardHelper;
    private readonly Robot _robot = new();

    #endregion

    #region Constructors

    public WindowsPlatformCapabilities(WindowsPlatformDependencyResolver dependencyResolver)
    {
        _logger = dependencyResolver.Logger;
        _clipboardHelper = dependencyResolver.ClipboardHelper;
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return Task.FromResult(User32.SetForegroundWindow(process.MainWindowHandle));
    }

    public Task<bool> SetClipboardTextAsync(string text)
    {
        return _clipboardHelper.SetClipboardTextAsync(text);
    }

    public Task<string?> GetClipboardTextAsync()
    {
        return _clipboardHelper.GetClipboardTextAsync();
    }

    public Task<bool> ResetClipboardTextAsync()
    {
        return _clipboardHelper.ResetClipboardTextAsync();
    }

    public string? GetGameFolder(Process gameProcess)
    {
        return gameProcess.MainModule is null ? null : Path.GetDirectoryName(gameProcess.MainModule.FileName);
    }

    public Task<bool> SendKeyboardPasteAsync()
    {
        _robot.KeyDown(Key.Control);
        _robot.KeyPress(Key.V);
        _robot.KeyUp(Key.Control);
        return Task.FromResult(true);
    }

    #endregion
}