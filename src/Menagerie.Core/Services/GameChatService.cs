using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class GameChatService : IGameChatService
{
    #region Public methods

    public Task SendBusyWhisper(Trade trade)
    {
        throw new NotImplementedException();
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