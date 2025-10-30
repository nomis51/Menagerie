using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Services.Abstractions;

public interface IGameChatService
{
    Task SendBusyWhisper(Trade trade);
    Task PrepareToSendWhisper(Trade trade);
    Task SendSoldWhisper(Trade trade);
    Task SendStillInterestedWhisper(Trade trade);
    Task SendInviteCommand(Trade trade);
    Task SendKickCommand(Trade trade);
    Task SendTradeRequestCommand(Trade trade);
}