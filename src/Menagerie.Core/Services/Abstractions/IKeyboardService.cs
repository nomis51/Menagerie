namespace Menagerie.Core.Services.Abstractions;

public interface IKeyboardService
{
    Task<bool> PasteAsync();
}