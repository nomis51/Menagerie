using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;
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

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _linuxLibs.FocusWindowAsync(process);
    }

    #endregion
}