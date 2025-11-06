using Desktop.Robot;
using Menagerie.Core.Services.Abstractions;
using Menagerie.Core.Shared.Abstractions;

namespace Menagerie.Core.Services;

public class KeyboardService : IKeyboardService
{
    #region Members

    // private  KeyboardHook? _hook;
    private readonly IPlatformCapabilities _platformCapabilities;

    #endregion

    #region Props

    // public  KeyboardState State { get; } = new();

    #endregion

    #region Constructors

    public KeyboardService(IPlatformCapabilities platformCapabilities)
    {
        _platformCapabilities = platformCapabilities;
    }

    #endregion

    #region Public methods

    public Task<bool> PasteAsync()
    {
        return _platformCapabilities.SendKeyboardPasteAsync();
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