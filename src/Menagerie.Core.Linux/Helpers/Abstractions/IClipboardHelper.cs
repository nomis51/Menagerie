namespace Menagerie.Core.Linux.Helpers.Abstractions;

public interface IClipboardHelper
{
    Task<bool> SetClipboardTextAsync(string text);
    Task<string?> GetClipboardTextAsync();
    Task<bool> ResetClipboardTextAsync();
}