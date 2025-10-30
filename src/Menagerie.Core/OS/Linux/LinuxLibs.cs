using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;
using Menagerie.Core.OS.Linux.Wayland;
using Menagerie.Core.OS.Linux.X11;

namespace Menagerie.Core.OS.Linux;

public class LinuxLibs : ILinuxLibs
{
    #region Constants

    private const string X11SessionType = "x11";
    private const string WaylandSessionType = "wayland";

    #endregion

    #region Members

    private readonly X11Lib _x11Lib = new();
    private readonly WaylandLib _waylandLib = new();

    #endregion

    #region Public methods

    public async Task<bool> FocusWindowAsync(Process process)
    {
        var sessionType = GetSessionType();
        return sessionType switch
        {
            X11SessionType => _x11Lib.FocusWindow(process.MainWindowHandle),
            WaylandSessionType => await _waylandLib.FocusWindowAsync(process),
            _ => false
        };
    }

    #endregion

    #region Private methods

    private static string? GetSessionType()
    {
        return Environment.GetEnvironmentVariable("XDG_SESSION_TYPE");
    }

    #endregion
}