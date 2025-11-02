using System.Diagnostics;
using Menagerie.Core.Linux.Compositors;
using Menagerie.Core.Linux.Compositors.Abstractions;
using Menagerie.Core.Linux.Helpers.Abstractions;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Linux.Platforms;

public class WaylandPlatformCapabilities : IPlatformCapabilities
{
    #region Members

    private readonly ILinuxCompositor _compositor = GetCompositor();
    private readonly IClipboardHelper _clipboardHelper;

    #endregion

    #region Constructors

    public WaylandPlatformCapabilities(WaylandPlatformDependencyResolver dependencyResolver)
    {
        _clipboardHelper = dependencyResolver.ClipboardHelper;
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _compositor.FocusWindowAsync(process);
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

    #endregion

    #region Private methods

    private static ILinuxCompositor GetCompositor()
    {
        var desktopType = Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP");
        if (desktopType is null) throw new PlatformNotSupportedException("Unknown desktop type");

        return desktopType.ToLower() switch
        {
            "hyprland" => new HyprlandCompositor(),
            _ => throw new PlatformNotSupportedException("Unknown desktop type")
        };
    }

    #endregion
}