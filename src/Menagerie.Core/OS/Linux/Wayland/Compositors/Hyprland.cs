using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;

namespace Menagerie.Core.OS.Linux.Wayland.Compositors;

public class Hyprland : ICompositor
{
    #region Public methods

    public async Task<bool> FocusWindowAsync(Process process)
    {
        var p = Process.Start("hyprctl", $"dispatch focuswindow pid:{process.Id}");
        await p.WaitForExitAsync();
        return p.ExitCode == 0;
    }

    #endregion
}