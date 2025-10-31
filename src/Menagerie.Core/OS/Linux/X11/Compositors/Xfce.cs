using System.Diagnostics;
using Menagerie.Core.OS.Linux.Abstractions;

namespace Menagerie.Core.OS.Linux.X11.Compositors;

public class Xfce : ICompositor
{
    #region Public methods

    public Task<bool> FocusWindowAsync(Process process)
    {
        throw new NotImplementedException();
    }

    #endregion
}