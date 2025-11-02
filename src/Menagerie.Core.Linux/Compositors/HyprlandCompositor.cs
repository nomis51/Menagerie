using System.Diagnostics;
using Menagerie.Core.Linux.Compositors.Abstractions;

namespace Menagerie.Core.Linux.Compositors;

public class HyprlandCompositor : ILinuxCompositor
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