using System.Diagnostics;

namespace Menagerie.Core.Shared.Abstractions;

public interface IPlatformCapabilities
{
    Task<bool> FocusWindowAsync(Process process);
    Task<bool> SetClipboardTextAsync(string text);
    Task<string?> GetClipboardTextAsync();
    Task<bool> ResetClipboardTextAsync();
}