using Menagerie.Core.Services.Abstractions;
using Serilog;

namespace Menagerie.Core.Services;

public class WindowHookService : IWindowHookService
{
    #region Members

    private KeyboardHook? _keyboardHook;
    // private MouseHook _mouseHook;

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public void IoHook(int processId)
    {
        _keyboardHook = new KeyboardHook(processId);
        _keyboardHook.AddHandler(KeyCode.F4, KeyboardHook_OnSearchOutgoingOffer);
        _keyboardHook.AddHandler(KeyCode.F3, Keyboard_OnToggleOverlay);
        _keyboardHook.AddHandler(KeyCode.F6, Keyboard_OnSaveClip);
        _keyboardHook.AddHandler(KeyCode.F, Modifiers.Control, KeyboardHook_OnSearchItemInStash);

        try
        {
            _ = _keyboardHook.InstallAsync();
        }
        catch (Exception e)
        {
            Log.Warning("Winook error: {Message}", e.Message);
        }

        // _mouseHook = new MouseHook(processId);
        // _mouseHook.MessageReceived += MouseHookOnMessageReceived;
        // _ = _mouseHook.InstallAsync();
    }

    #endregion

    #region Private methods

    private static void Keyboard_OnSaveClip(object? sender, KeyboardMessageEventArgs e)
    {
        AppDataService.Instance.SaveLastClip();
    }

    private static void Keyboard_OnToggleOverlay(object? sender, KeyboardMessageEventArgs e)
    {
        AppDataService.Instance.ToggleOverlay();
    }

    private static void KeyboardHook_OnSearchOutgoingOffer(object? sender, KeyboardMessageEventArgs e)
    {
        AppDataService.Instance.OnSearchOutgoingOffer();
    }

    private static void KeyboardHook_OnSearchItemInStash(object? sender, KeyboardMessageEventArgs e)
    {
        AppDataService.Instance.OnSearchItemInStash();
    }

    #endregion
}