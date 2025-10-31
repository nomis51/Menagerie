namespace Menagerie.Core.Services.Abstractions;

public interface IClipboardService
{
    Task<string?> GetTextAsync();
    Task<bool> SetTextAsync(string text);
    Task<bool> ResetTextAsync();
}