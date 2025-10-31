using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;

namespace Menagerie.Core.OS.Linux.X11.Compositors;

public class DefaultCompositor : ICompositor
{
    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        return Task.FromResult(false);
    }

    #endregion
}