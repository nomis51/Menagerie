using System;
using System.Threading;

namespace Menagerie.Helpers;

public class TextBoxDebouncer
{
    #region Members

    public event EventHandler OnIdled = delegate { };
    private readonly Timer _waitingTimer;
    private readonly int _waitingMilliSeconds;

    #endregion

    #region Props

    #endregion

    #region Constructors

    public TextBoxDebouncer(int waitingMilliSeconds = 500)
    {
        _waitingMilliSeconds = waitingMilliSeconds;
        _waitingTimer = new Timer(_ => { OnIdled(this, EventArgs.Empty); });
    }

    #endregion

    #region Public methods

    public void TextChanged()
    {
        _waitingTimer.Change(_waitingMilliSeconds, Timeout.Infinite);
    }

    #endregion
}