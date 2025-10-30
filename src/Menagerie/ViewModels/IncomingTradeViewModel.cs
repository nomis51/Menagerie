using Menagerie.Core.Enums;
using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie.ViewModels;

public class IncomingTradeViewModel : ViewModelBase
{
    #region Props

    public IKeyboardService KeyboardService { get; }
    public IAudioService AudioService { get; }
    public int TileSize { get; }
    public Trade Trade { get; }

    #endregion

    #region Constructors

    public IncomingTradeViewModel(IKeyboardService keyboardService, int tileSize, IAudioService audioService)
    {
        KeyboardService = keyboardService;
        TileSize = tileSize;
        AudioService = audioService;
    }

    #endregion

    #region Public methods

    public void DoNextAction()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            SendInvitePlayer();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
        else if (!Trade.State.HasFlag(TradeState.Trading))
        {
            SendTrade();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
    }

    public void SayBusy()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.Busy;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendBusyWhisper(Trade);
    }

    public void Whisper()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        AppService.Instance.PrepareToSendWhisper(Trade);
    }

    public void SaySold()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        AppService.Instance.SendSoldWhisper(Trade);
        SendDenyOffer();
    }

    public void AskStillInterested()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.StillInterested;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendStillInterestedWhisper(Trade);
    }

    public void SendInvitePlayer()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;

        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            Trade.State |= TradeState.PlayerInvited;
            this.RaisePropertyChanged(nameof(BorderBrush));

            AppService.Instance.SendInviteCommand(Trade);
        }
        else
        {
            AppService.Instance.SendReInviteCommand(Trade);
        }
    }

    public void SendDenyOffer()
    {
        AudioService.PlayEffectAsync(AudioEffect.Click);

        if (Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            AppService.Instance.SendKickCommand(Trade);
        }

        Trade.State = TradeState.Done;
        this.RaisePropertyChanged(nameof(BorderBrush));

        OnRemoved?.Invoke(Trade.Id);
    }

    #endregion
}