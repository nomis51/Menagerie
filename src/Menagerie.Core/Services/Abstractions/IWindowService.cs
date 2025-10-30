namespace Menagerie.Core.Services.Abstractions;

public interface IWindowService
{
    Task<bool> FocusWindowAsync(IntPtr hwnd);
}