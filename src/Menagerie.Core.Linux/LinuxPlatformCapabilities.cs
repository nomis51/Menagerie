using System.Diagnostics;
using Menagerie.Core.Linux.Platforms;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Linux;

public class LinuxPlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly IPlatformCapabilities _platformCapabilities;

    #endregion

    #region Constructors

    public LinuxPlatformCapabilities(LinuxPlatformDependencyResolver dependencyResolver)
    {
        _platformCapabilities = GetPlatformCapabilities(dependencyResolver);
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _platformCapabilities.FocusWindowAsync(process);
    }

    public Task<bool> SetClipboardTextAsync(string text)
    {
        return _platformCapabilities.SetClipboardTextAsync(text);
    }

    public Task<string?> GetClipboardTextAsync()
    {
        return _platformCapabilities.GetClipboardTextAsync();
    }

    public Task<bool> ResetClipboardTextAsync()
    {
        return _platformCapabilities.ResetClipboardTextAsync();
    }

    #endregion

    #region Private methods

    private IPlatformCapabilities GetPlatformCapabilities(LinuxPlatformDependencyResolver dependencyResolver)
    {
        var sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
        return sessionType switch
        {
            "wayland" => new WaylandPlatformCapabilities(dependencyResolver.WaylandPlatformDependencyResolver),
            "x11" => new X11PlatformCapabilities(),
            _ => throw new PlatformNotSupportedException("Unknown session type")
        };
    }

    #endregion
}