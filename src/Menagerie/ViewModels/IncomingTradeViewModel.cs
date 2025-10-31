using System;
using System.Globalization;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Menagerie.Core.Enums;
using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie.ViewModels;

public class IncomingTradeViewModel : ViewModelBase
{
    #region Events

    public EventHandler<object?>? Removed { get; set; }

    #endregion

    #region Members

    private IAudioService AudioService { get; }
    private IGameChatService GameChatService { get; }

    public int TileSize { get; }
    public Trade Trade { get; }

    #endregion

    #region Props

    public bool CanSayBusy => !Trade.State.HasFlag(TradeState.PlayerInvited);

    public IBrush BorderBrush
    {
        get
        {
            if (Trade.State.HasFlag(TradeState.Done))
                return new SolidColorBrush((Color)Application.Current!.Resources["ErrorColor"]!);
            if (Trade.State.HasFlag(TradeState.Trading))
                return new SolidColorBrush((Color)Application.Current!.Resources["WarningColor"]!);
            if (Trade.State.HasFlag(TradeState.PlayerInvited))
                return new SolidColorBrush((Color)Application.Current!.Resources["SuccessColor"]!);
            if (Trade.State.HasFlag(TradeState.Busy))
                return new SolidColorBrush((Color)Application.Current!.Resources["AccentColor"]!);

            return new SolidColorBrush((Color)Application.Current!.Resources["Background0"]!);
        }
    }

    public Task<Bitmap?> CurrencyImage => Task.FromResult<Bitmap?>(null);

    public int PriceQuantityFontSize
    {
        get
        {
            var str = Trade.Price.ToString(CultureInfo.InvariantCulture);
            var hasDot = str.Contains('.', StringComparison.Ordinal);
            str = str.Replace(".", string.Empty);

            return str.Length switch
            {
                1 => 28,
                2 when !hasDot => 22,
                2 => 21,
                3 when !hasDot => 17,
                3 => 16,
                4 when !hasDot => 15,
                4 => 13,
                _ => 1
            };
        }
    }

    public int PriceQuantityColspan => PriceQuantityFontSize <= 15 ? 2 : 1;
    public int PriceQuantityColumn => PriceQuantityFontSize <= 15 ? 0 : 1;

    public HorizontalAlignment PriceQuantityHorizontalAlignment =>
        PriceQuantityFontSize <= 15 ? HorizontalAlignment.Right : HorizontalAlignment.Center;

    private bool _isPlayerInTheArea;

    public bool IsPlayerInTheArea
    {
        get => _isPlayerInTheArea;
        set
        {
            _isPlayerInTheArea = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region Constructors

    public IncomingTradeViewModel(
        int tileSize,
        IAudioService audioService,
        IGameChatService gameChatService,
        Trade trade
    )
    {
        TileSize = tileSize;
        AudioService = audioService;
        GameChatService = gameChatService;
        Trade = trade;
    }

    #endregion

    #region Public methods

    public async Task DoNextAction()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            await SendInvitePlayer();
            OnPropertyChanged(nameof(CanSayBusy));
        }
        else if (!Trade.State.HasFlag(TradeState.Trading))
        {
            await SendTrade();
            OnPropertyChanged(nameof(CanSayBusy));
        }
    }

    public async Task SayBusy()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.Busy;
        OnPropertyChanged(nameof(BorderBrush));

        await GameChatService.SendBusyWhisper(Trade);
    }

    public async Task Whisper()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        await GameChatService.PrepareToSendWhisper(Trade);
    }

    public async Task SaySold()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        await GameChatService.SendSoldWhisper(Trade);
        await SendDenyOffer();
    }

    public async Task AskStillInterested()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;
        Trade.State |= TradeState.StillInterested;
        OnPropertyChanged(nameof(BorderBrush));

        await GameChatService.SendStillInterestedWhisper(Trade);
    }

    public async Task SendInvitePlayer()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        Trade.State &= ~TradeState.Initial;

        if (!Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            Trade.State |= TradeState.PlayerInvited;
            OnPropertyChanged(nameof(BorderBrush));
        }

        await GameChatService.SendInviteCommand(Trade);
    }

    public async Task SendDenyOffer()
    {
        await AudioService.PlayEffectAsync(AudioEffect.Click);

        if (Trade.State.HasFlag(TradeState.PlayerInvited))
        {
            await GameChatService.SendKickCommand(Trade);
        }

        Trade.State = TradeState.Done;
        OnPropertyChanged(nameof(BorderBrush));

        Removed?.Invoke(this, null);
    }

    #endregion

    #region Private methods

    private async Task SendTrade()
    {
        Trade.State &= ~TradeState.StillInterested;
        Trade.State |= TradeState.Trading;
        OnPropertyChanged(nameof(BorderBrush));

        await GameChatService.SendTradeRequestCommand(Trade);
    }

    #endregion
}