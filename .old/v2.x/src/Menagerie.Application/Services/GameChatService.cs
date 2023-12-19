using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;

namespace Menagerie.Application.Services;

public class GameChatService : IService
{
    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public bool SendBusyWhisper(string player, string itemName)
    {
        var settings = AppService.Instance.GetSettings();
        return SendWhisper(player, RenderMessageTemplate(settings.IncomingTrades.BusyWhisper,
            new List<Tuple<string, string>>
            {
                new("{item}", itemName),
                new("{location}", string.IsNullOrEmpty(AppService.Instance.GetCurrentLocation()) ? "Unknown location" : AppService.Instance.GetCurrentLocation())
            }));
    }

    public bool SendSoldWhisper(string player, string itemName)
    {
        var settings = AppService.Instance.GetSettings();
        return SendWhisper(player, RenderMessageTemplate(settings.IncomingTrades.SoldWhisper,
            new List<Tuple<string, string>>
            {
                new("{item}", itemName)
            }));
    }

    public bool SendStillInterestedWhisper(string player, string itemName, string price)
    {
        var settings = AppService.Instance.GetSettings();
        if (string.IsNullOrEmpty(settings.IncomingTrades.InviteWhisper)) return true;

        return SendWhisper(player, RenderMessageTemplate(settings.IncomingTrades.StillInterestedWhisper,
            new List<Tuple<string, string>>
            {
                new("{item}", itemName),
                new("{price}", price)
            }));
    }

    public bool SendInviteWhisper(string player, string itemName, string price)
    {
        var settings = AppService.Instance.GetSettings();
        return SendWhisper(player, RenderMessageTemplate(settings.IncomingTrades.InviteWhisper,
            new List<Tuple<string, string>>
            {
                new("{item}", itemName),
                new("{price}", price)
            }));
    }

    public bool SendThanksWhisper(string player)
    {
        var settings = AppService.Instance.GetSettings();
        return SendWhisper(player, settings.IncomingTrades.ThanksWhisper);
    }

    public bool SendInvite(string player)
    {
        return Send($"/invite {player}");
    }

    public bool SendKick(string player)
    {
        return Send($"/kick {player}");
    }

    public bool SendReInvite(string player)
    {
        return SendKick(player) && SendInvite(player);
    }

    public bool SendTradeRequest(string player)
    {
        return Send($"/tradewith {player}");
    }

    public bool SendHideoutCommand(string player = "")
    {
        return Send(string.IsNullOrEmpty(player) ? "/hideout" : $"/hideout {player}");
    }

    public bool SendFindItemInStash(string textQuery)
    {
        if (!AppService.Instance.EnsureGameFocused()) return false;

        ClipboardHelper.SetClipboard(textQuery, 100);

        KeyboardHelper.ClearModifiers();
        KeyboardHelper.SendControlF();
        KeyboardHelper.SendControlV();

        ClipboardHelper.ResetClipboardValue();

        return true;
    }

    public bool PrepareToSendWhisper(string player)
    {
        return Send($"@{player} ", false);
    }

    public bool Send(string message, bool doSend = true)
    {
        if (!AppService.Instance.EnsureGameFocused()) return false;

        ClipboardHelper.SetClipboard(message);
        KeyboardHelper.SendEnter();
        KeyboardHelper.SendControlA();
        KeyboardHelper.SendControlV();

        if (doSend)
        {
            KeyboardHelper.SendEnter();
        }

        ClipboardHelper.ResetClipboardValue();

        return true;
    }

    public bool SendWhisper(string player, string message)
    {
        return Send($"@{player} {message}");
    }

    #endregion

    #region Private methods

    private string RenderMessageTemplate(string message, List<Tuple<string, string>> args)
    {
        return args.Aggregate(message, (current, arg) => current.Replace(arg.Item1, arg.Item2));
    }

    #endregion
}