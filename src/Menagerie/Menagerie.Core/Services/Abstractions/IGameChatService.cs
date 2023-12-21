namespace Menagerie.Core.Services.Abstractions;

public interface IGameChatService : IService
{
    bool SendBusyWhisper(string player, string itemName);
    bool SendSoldWhisper(string player, string itemName);
    bool SendStillInterestedWhisper(string player, string itemName, string price);
    bool SendInviteWhisper(string player, string itemName, string price);
    bool SendThanksWhisper(string player);
    bool SendInvite(string player);
    bool SendKick(string player);
    bool SendReInvite(string player);
    bool SendTradeRequest(string player);
    bool SendHideoutCommand(string player = "");
    bool SendFindItemInStash(string textQuery);
    bool PrepareToSendWhisper(string player);
    bool Send(string message, bool doSend = true);
    bool SendWhisper(string player, string message);
}