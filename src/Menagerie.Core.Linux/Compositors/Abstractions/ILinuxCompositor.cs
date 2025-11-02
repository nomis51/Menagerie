using System.Diagnostics;

namespace Menagerie.Core.Linux.Compositors.Abstractions;

public interface ILinuxCompositor
{
    Task<bool> FocusWindowAsync(Process process);
}