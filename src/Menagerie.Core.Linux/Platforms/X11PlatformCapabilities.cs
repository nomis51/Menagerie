using System.Diagnostics;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Linux.Platforms;

public class X11PlatformCapabilities : IPlatformCapabilities
{
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

    #endregion
}