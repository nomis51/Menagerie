namespace Menagerie.Core.Linux.Helpers.Abstractions;

public interface IKeyboardHelper
{
    Task<bool> SendEnterAsync();
    Task<bool> SendPasteAsync();
}