using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Menagerie.Core;
using Menagerie.Core.Services;
using Menagerie.Enums;
using Menagerie.Models;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOfferViewModel : ViewModelBase
{
    #region Events

    public delegate void RemovedEvent(string id);

    public event RemovedEvent? Removed;

    #endregion

    #region Props

    public OfferModel Offer { get; set; }

    public int PriceQuantityFontSize
    {
        get
        {
            var str = Offer.Price.Quantity.ToString(CultureInfo.InvariantCulture);
            var hasDot = str.Contains('.', StringComparison.Ordinal);
            str = str.Replace(".", string.Empty);

            return str.Length switch
            {
                1 => 24,
                2 when !hasDot => 22,
                2 => 21,
                3 when !hasDot => 18,
                3 => 17,
                4 when !hasDot => 15,
                4 => 13,
                _ => 1
            };
        }
    }

    public ITransform? PriceQuantityTransform => PriceQuantityFontSize <= 13 ? TransformOperations.Parse("rotate(-35deg)") : null;

    public IBrush BorderBrush
    {
        get
        {
            if (Offer.State.HasFlag(OfferState.Done)) return new SolidColorBrush((Color)Application.Current!.Resources["ErrorColor"]!);
            if (Offer.State.HasFlag(OfferState.Trading)) return new SolidColorBrush((Color)Application.Current!.Resources["WarningColor"]!);
            if (Offer.State.HasFlag(OfferState.PlayerInvited)) return new SolidColorBrush((Color)Application.Current!.Resources["SuccessColor"]!);
            if (Offer.State.HasFlag(OfferState.Busy)) return new SolidColorBrush((Color)Application.Current!.Resources["AccentColor"]!);

            return new SolidColorBrush((Color)Application.Current!.Resources["Background0"]!);
        }
    }

    public ObservableCollection<Tuple<string, string>> TooltipLines { get; private set; } = [];
    public bool CanSayBusy => !Offer.State.HasFlag(OfferState.PlayerInvited);

    private bool _isPlayerInTheArea;

    public bool IsPlayerInTheArea
    {
        get => _isPlayerInTheArea;
        set => this.RaiseAndSetIfChanged(ref _isPlayerInTheArea, value);
    }

    public int OfferSize;

    #endregion

    #region Constructors

    public IncomingOfferViewModel(OfferModel offer, int size)
    {
        Offer = offer;
        OfferSize = size;

        GenerateTooltip();

        Events.PlayerJoined += Events_OnPlayerJoined;
        Events.TradeAccepted += Events_OnTradeAccepted;
        Events.TradeCancelled += Events_OnTradeCancelled;
    }

    #endregion

    #region Public methods

    public void DoNextAction()
    {
        if (!Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            InvitePlayer();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
        else if (!Offer.State.HasFlag(OfferState.Trading))
        {
            Trade();
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
    }

    public void SayBusy()
    {
        Offer.State &= ~OfferState.Initial;
        Offer.State |= OfferState.Busy;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendBusyWhisper(Offer.Player, Offer.Item);
    }

    public void Whisper()
    {
        AppService.Instance.PrepareToSendWhisper(Offer.Player);
    }

    public void SaySold()
    {
        AppService.Instance.SendSoldWhisper(Offer.Player, Offer.Item);
        DenyOffer();
    }

    public void AskStillInterested()
    {
        Offer.State &= ~OfferState.Initial;
        Offer.State |= OfferState.StillInterested;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendStillInterestedWhisper(Offer.Player, Offer.Item, $"{Offer.Price.Quantity} {Offer.Price.Currency}");
    }

    public void InvitePlayer()
    {
        Offer.State &= ~OfferState.Initial;

        if (!Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            Offer.State |= OfferState.PlayerInvited;
            this.RaisePropertyChanged(nameof(BorderBrush));

            AppService.Instance.SendInviteCommand(Offer.Player, Offer.Item, $"{Offer.Price.Quantity} {Offer.Price.Currency}");
        }
        else
        {
            AppService.Instance.SendReInvitecommand(Offer.Player);
        }
    }

    public void DenyOffer()
    {
        Offer.State = OfferState.Done;
        this.RaisePropertyChanged(nameof(BorderBrush));

        if (Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            AppService.Instance.SendKickCommand(Offer.Player);
        }

        Removed?.Invoke(Offer.Id);
    }

    #endregion

    #region Private methdos

    private void Trade()
    {
        Offer.State &= ~OfferState.StillInterested;
        Offer.State |= OfferState.Trading;
        this.RaisePropertyChanged(nameof(BorderBrush));

        AppService.Instance.SendTradeRequestCommand(Offer.Player);
    }

    private void Events_OnTradeCancelled()
    {
        if (!Offer.State.HasFlag(OfferState.Trading)) return;

        Offer.State &= ~OfferState.Trading;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    private void Events_OnTradeAccepted()
    {
        if (!Offer.State.HasFlag(OfferState.Trading)) return;

        AppService.Instance.SendThanksWhisper(Offer.Player);
        DenyOffer();
    }

    private void Events_OnPlayerJoined(string player)
    {
        if (IsPlayerInTheArea) return;
        if (Offer.Player != player || Offer.State.HasFlag(OfferState.PlayerJoined)) return;

        Offer.State &= ~OfferState.StillInterested;
        Offer.State |= OfferState.PlayerJoined;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    private void GenerateTooltip()
    {
        TooltipLines.Add(new Tuple<string, string>("Time : ", Offer.Time.ToLongTimeString()));
        TooltipLines.Add(new Tuple<string, string>("Player : ", Offer.Player));
        TooltipLines.Add(new Tuple<string, string>("Item : ", $"{(Offer.Quantity > 0 ? $"{Offer.Quantity}x " : string.Empty)}{Offer.Item}"));
        TooltipLines.Add(new Tuple<string, string>("Price : ", $"{Offer.Price.Quantity} {Offer.Price.Currency}"));
        TooltipLines.Add(new Tuple<string, string>("League : ", Offer.League));
        TooltipLines.Add(new Tuple<string, string>("Location : ", $"{Offer.Location.StashTab} (Left: {Offer.Location.Left}, Top: {Offer.Location.Top})"));
    }

    #endregion
}