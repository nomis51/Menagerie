using System.Diagnostics;
using Desktop.Robot;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Linux.Platforms;

public class X11PlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly Robot _robot = new();

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetClipboardTextAsync(string text)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetClipboardTextAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> ResetClipboardTextAsync()
    {
        throw new NotImplementedException();
    }

    public string? GetGameFolder(Process gameProcess)
    {
        throw new NotSupportedException();
    }

    public Task<bool> SendKeyboardPasteAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendKeyboardEnterAsync()
    {
        throw new NotImplementedException();
    }

    #endregion
}