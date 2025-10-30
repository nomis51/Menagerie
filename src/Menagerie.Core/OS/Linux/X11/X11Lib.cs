using System.Runtime.InteropServices;

namespace Menagerie.Core.OS.Linux.X11;

public class X11Lib
{
    #region Constants

    private const int RevertToParent = 2;

    #endregion

    #region Public methods

    public bool FocusWindow(IntPtr hwnd)
    {
        var display = XOpenDisplay(IntPtr.Zero);
        if (display == IntPtr.Zero) return false;

        XRaiseWindow(display, hwnd);
        XSetInputFocus(display, hwnd, RevertToParent, 0);
        XCloseDisplay(display);

        return true;
    }

    #endregion

    #region Imports

    [DllImport("libX11.so")]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport("libX11.so")]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport("libX11.so")]
    private static extern void XSetInputFocus(IntPtr display, IntPtr window, int revertTo, int time);

    [DllImport("libX11.so")]
    private static extern void XRaiseWindow(IntPtr display, IntPtr window);

    #endregion
}