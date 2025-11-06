using System.Runtime.InteropServices;
using Menagerie.Core.Linux.Helpers.Abstractions;

namespace Menagerie.Core.Linux.Helpers;

public class KeyboardHelper : IKeyboardHelper
{
    #region Constants

    private const string Libx11 = "libX11.so.6";
    private const string Libxtst = "libXtst.so.6";

    #endregion

    #region Imports

    [DllImport(Libx11)]
    private static extern IntPtr XOpenDisplay(IntPtr display);

    [DllImport(Libx11)]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport(Libxtst)]
    private static extern int XTestFakeKeyEvent(IntPtr display, uint keycode, bool is_press, ulong delay);

    [DllImport(Libx11)]
    private static extern int XFlush(IntPtr display);

    [DllImport(Libx11)]
    private static extern uint XKeysymToKeycode(IntPtr display, ulong keysym);

    #endregion

    #region Keys

    private const ulong XkReturn = 0xff0dUL;
    private const ulong XkControlLeft = 0xffe3UL;
    private const ulong XkV = 0x0076UL;

    #endregion

    #region Public methods

    public Task<bool> SendEnterAsync()
    {
        SendKeyPress(XkReturn);
        return Task.FromResult(true);
    }

    public Task<bool> SendPasteAsync()
    {
        SendKeyPress(XkV, XkControlLeft);
        return Task.FromResult(true);
    }

    #endregion

    #region Private methods

    private static void SendKeyPress(ulong keysym, ulong modifierKeysym = 0)
    {
        var dpy = XOpenDisplay(IntPtr.Zero);
        if (dpy == IntPtr.Zero)
        {
            throw new InvalidOperationException("Unable to open X display");
        }

        try
        {
            var kcModifier = 0u;
            if (modifierKeysym != 0)
            {
                kcModifier = KeysymToKeycode(dpy, modifierKeysym);
                KeyDown(dpy, kcModifier);
            }

            var kcEnter = KeysymToKeycode(dpy, keysym);
            KeyDown(dpy, kcEnter);
            KeyUp(dpy, kcEnter);

            if (kcModifier != 0)
            {
                KeyUp(dpy, kcModifier);
            }
        }
        finally
        {
            XCloseDisplay(dpy);
        }
    }

    private static uint KeysymToKeycode(IntPtr dpy, ulong keysym)
    {
        var kc = XKeysymToKeycode(dpy, keysym);
        return kc == 0 ? throw new InvalidOperationException($"No keycode for keysym 0x{keysym:X}") : kc;
    }

    private static void KeyDown(IntPtr dpy, uint keycode)
    {
        XTestFakeKeyEvent(dpy, keycode, true, 0);
        XFlush(dpy);
    }

    private static void KeyUp(IntPtr dpy, uint keycode)
    {
        XTestFakeKeyEvent(dpy, keycode, false, 0);
        XFlush(dpy);
    }

    #endregion
}