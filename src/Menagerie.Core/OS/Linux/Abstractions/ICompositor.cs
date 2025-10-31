using System.Diagnostics;

namespace Menagerie.Core.OS.Linux.Abstractions;

public interface ICompositor
{
    Task<bool> FocusWindowAsync(Process process);
}