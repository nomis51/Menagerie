using System.Diagnostics;
using Menagerie.Core.Enums.OS;

namespace Menagerie.Core.OS.Linux.Wayland;

public class WaylandLib
{
    #region Members

    private CompositorName _compositorName = CompositorName.Undefined;

    #endregion

    #region Public methods

    public async Task<bool> FocusWindowAsync(IntPtr hwnd)
    {
        await EnsureCompositorFound();

        return _compositorName switch
        {
            CompositorName.Hyprland => await FocusWindowHyprlandAsync(hwnd),
            CompositorName.Kde => await FocusWindowKdeAsync(hwnd),
            CompositorName.Xdotool => await FocusWindowFallbackAsync(hwnd),
            _ => false
        };
    }

    #endregion

    #region Private methods

    private async Task EnsureCompositorFound()
    {
        if (_compositorName != CompositorName.Undefined) return;

        if (await IsHyprlandCompositor())
        {
            _compositorName = CompositorName.Hyprland;
        }
        else if (await IsKdeCompositor())
        {
            _compositorName = CompositorName.Kde;
        }
        else if (await IsXdotoolCompositor())
        {
            _compositorName = CompositorName.Xdotool;
        }
    }

    private static async Task<bool> FocusWindowHyprlandAsync(IntPtr hwnd)
    {
        var process = Process.Start("hyprctl", $"dispatch focuswindow address:{hwnd}");
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }

    private static async Task<bool> FocusWindowKdeAsync(IntPtr hwnd)
    {
        var process = Process.Start("qdbus", $"org.kde.KWin /KWin org.kde.KWin.activateWindow {hwnd}");
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
    }

    private static async Task<bool> FocusWindowFallbackAsync(IntPtr hwnd)
    {
        var process = Process.Start("xdotool", $"windowactivate {hwnd}");
        await process.WaitForExitAsync();
        return process.ExitCode == 0;
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