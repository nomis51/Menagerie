using System.Runtime.InteropServices;

namespace Menagerie.Core.OS.Win32;

public class User32
{
    #region Public methods

    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    #endregion
}