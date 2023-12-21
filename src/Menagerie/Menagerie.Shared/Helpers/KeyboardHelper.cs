using Desktop.Robot;

namespace Menagerie.Shared.Helpers;

public static class KeyboardHelper
{
    #region Members

    private static readonly Robot Robot = new();

    #endregion

    #region Public methods

    public static void ClearModifiers()
    {
        Robot.KeyUp(Key.Shift);
        Robot.KeyUp(Key.Control);
        Robot.KeyUp(Key.Alt);
    }

    public static void ClearKey(Key key)
    {
        Robot.KeyUp(key);
    }

    public static void SendControlA()
    {
        ControlledKeyPress(Key.A);
    }

    public static void SendControlV()
    {
        ControlledKeyPress(Key.V);
    }

    public static void SendControlC()
    {
        ControlledKeyPress(Key.C);
    }

    public static void SendControlF()
    {
        ControlledKeyPress(Key.F);
    }

    public static void SendEnter()
    {
        Robot.KeyPress(Key.Enter);
    }

    public static void SendEscape()
    {
        Robot.KeyPress(Key.Esc);
    }

    public static void SendDelete()
    {
        Robot.KeyPress(Key.Delete);
    }

    #endregion

    #region Private methods

    private static void ControlledKeyPress(Key key)
    {
        Robot.KeyDown(Key.Control);
        Robot.KeyPress(key);
        Robot.KeyUp(Key.Control);
    }

    #endregion
}