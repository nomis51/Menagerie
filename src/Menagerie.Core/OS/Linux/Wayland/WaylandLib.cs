using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;
using Menagerie.Core.OS.Linux.Wayland.Compositors;

namespace Menagerie.Core.OS.Linux.Wayland;

public class WaylandLib
{
    #region Members

    private ICompositor? _compositor;

    #endregion

    #region Constructors

    public WaylandLib()
    {
        DetectCompositor();
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _compositor is null ? Task.FromResult(false) : _compositor.FocusWindowAsync(process);
    }

    #endregion

    #region Private methods

    private void DetectCompositor()
    {
        Task.Run(async () => _compositor = await DetectCompositorAsync());
    }

    private static async Task<ICompositor> DetectCompositorAsync()
    {
        if (await IsHyprlandCompositor())
        {
            return new Hyprland();
        }

        if (await IsKdeCompositor())
        {
        }
        else if (await IsXdotoolCompositor())
        {
        }

        return new DefaultCompositor();
    }

    private static Task<bool> IsHyprlandCompositor()
    {
        return IsCommandAvailable("hyprctl");
    }

    private static Task<bool> IsKdeCompositor()
    {
        return IsCommandAvailable("qdbus");
    }

    private static Task<bool> IsXdotoolCompositor()
    {
        return IsCommandAvailable("xdotool");
    }

    private static async Task<bool> IsCommandAvailable(string cmd)
    {
        try
        {
            ProcessStartInfo psi = new("which", cmd)
            {
                RedirectStandardOutput = true
            };
            var proc = Process.Start(psi);
            if (proc is null) return false;

            await proc.WaitForExitAsync();
            return proc.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}