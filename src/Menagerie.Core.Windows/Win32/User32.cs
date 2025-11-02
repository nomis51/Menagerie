using System.Runtime.InteropServices;

namespace Menagerie.Core.Windows.Win32;

public static class User32
{
    #region Imports

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    #endregion
}