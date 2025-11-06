using Desktop.Robot;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class GameChatService : IGameChatService
{
    #region Members

    private readonly IClipboardService _clipboardService;
    private readonly IKeyboardService _keyboardService;
    private readonly IGameService _gameService;

    #endregion

    #region Constructors

    public GameChatService(
        IClipboardService clipboardService,
        IKeyboardService keyboardService,
        IGameService gameService
    )
    {
        _clipboardService = clipboardService;
        _keyboardService = keyboardService;
        _gameService = gameService;
    }

    #endregion

    #region Public methods

    public async Task SendBusyWhisper(Trade trade)
    {
        if (!await _clipboardService.SetTextAsync("im busy")) return;
        if (!await _gameService.FocusGameAsync()) return;

        await _keyboardService.PasteAsync();
    }

    public Task PrepareToSendWhisper(Trade trade)
    {
        throw new NotImplementedException();
    }

    public Task SendSoldWhisper(Trade trade)
    {
        throw new NotImplementedException();
    }

    public Task SendStillInterestedWhisper(Trade trade)
    {
        throw new NotImplementedException();
    }

    public Task SendInviteCommand(Trade trade)
    {
        throw new NotImplementedException();
    }

    public Task SendKickCommand(Trade trade)
    {
        throw new NotImplementedException();
    }

    public Task SendTradeRequestCommand(Trade trade)
    {
        throw new NotImplementedException();
    }

    #endregion
}