using System.Diagnostics;
using System.Runtime.InteropServices;
using Menagerie.Core.OS.Linux.Abstractions;
using Menagerie.Core.OS.Win32;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class WindowService : IWindowService
{
    #region Members

    private readonly ILinuxLibs _linuxLibs;

    #endregion

    #region Constructors

    public WindowService(ILinuxLibs linuxLibs)
    {
        _linuxLibs = linuxLibs;
    }

    #endregion

    #region Public methods

    public async Task<bool> FocusWindowAsync(Process process)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return User32.SetForegroundWindow(process.MainWindowHandle);
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return await _linuxLibs.FocusWindowAsync(process);
        }

        return false;
    }

    #endregion
}