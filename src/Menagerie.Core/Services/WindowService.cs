using System.Diagnostics;
using Menagerie.Core.Services.Abstractions;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Services;

public class WindowService : IWindowService
{
    #region Members

    private readonly IPlatformCapabilities _platformCapabilities;

    #endregion

    #region Constructors

    public WindowService(IPlatformCapabilities platformCapabilities)
    {
        _platformCapabilities = platformCapabilities;
    }

    #endregion

    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return _platformCapabilities.FocusWindowAsync(process);
    }

    #endregion
}