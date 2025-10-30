using System.Diagnostics;

namespace Menagerie.Core.Services.Abstractions;

public interface IWindowService
{
    Task<bool> FocusWindowAsync(Process process);
}