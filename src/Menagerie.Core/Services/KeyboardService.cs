using Desktop.Robot;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class KeyboardService : IKeyboardService
{
    #region Members

    // private  KeyboardHook? _hook;
    private readonly Robot _robot = new();

    #endregion

    #region Props

    // public  KeyboardState State { get; } = new();

    #endregion

    #region Public methods

    public void ClearModifiers()
    {
        _robot.KeyUp(Key.Shift);
        _robot.KeyUp(Key.Control);
        _robot.KeyUp(Key.Alt);
    }

    public void ClearKey(Key key)
    {
        _robot.KeyUp(key);
    }

    public void SendKey(Key key)
    {
        _robot.KeyPress(key);
    }

    public void SendKey(Key key, Key modifier)
    {
        _robot.KeyDown(modifier);
        SendKey(key);
        _robot.KeyUp(modifier);
    }

    #endregion

    #region Private methods

    // private  void OnMessageReceived(object? sender, KeyboardMessageEventArgs e)
    // {
    //     State.Control = e.Control;
    //     State.Shift = e.Shift;
    //     State.Alt = e.Alt;
    // }

    #endregion
}