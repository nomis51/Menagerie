namespace Menagerie.Core.OS.Linux.Abstractions;

public interface ILinuxLibs
{
    Task<bool> FocusWindowAsync(IntPtr hwnd);
}