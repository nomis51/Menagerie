using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Menagerie.Enums;
using Menagerie.Models;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOfferViewModel : ViewModelBase
{
    #region Props

    public OfferModel Offer { get; set; }
    public int Width { get; set; }

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
            if (Offer.State.HasFlag(OfferState.Trading)) return new SolidColorBrush((Color)Application.Current!.Resources["WarningColor"]!);
            if (Offer.State.HasFlag(OfferState.PlayerInvited)) return new SolidColorBrush((Color)Application.Current!.Resources["SuccessColor"]!);
            if (Offer.State.HasFlag(OfferState.Busy)) return new SolidColorBrush((Color)Application.Current!.Resources["AccentColor"]!);

            return new SolidColorBrush((Color)Application.Current!.Resources["Background0"]!);
        }
    }

    public ObservableCollection<Tuple<string, string>> TooltipLines { get; private set; } = [];
    public bool CanSayBusy => !Offer.State.HasFlag(OfferState.PlayerInvited);

    #endregion

    #region Constructors

    public IncomingOfferViewModel(OfferModel offer, int width = 50)
    {
        Offer = offer;
        Width = width;

        GenerateTooltip();
    }

    #endregion

    #region Public methods

    public void DoNextAction()
    {
        if (!Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            Offer.State = OfferState.PlayerInvited;
            this.RaisePropertyChanged(nameof(BorderBrush));
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
        else if (!Offer.State.HasFlag(OfferState.Trading))
        {
            Offer.State = OfferState.Trading | OfferState.PlayerInvited;
            this.RaisePropertyChanged(nameof(BorderBrush));
            this.RaisePropertyChanged(nameof(CanSayBusy));
        }
    }

    public void SayBusy()
    {
        Offer.State = OfferState.Busy;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    public void AskStillInterested()
    {
        if (Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            Offer.State |= OfferState.StillInterested;
            this.RaisePropertyChanged(nameof(BorderBrush));
        }
        else
        {
            Offer.State = OfferState.StillInterested;
            this.RaisePropertyChanged(nameof(BorderBrush));
        }
    }

    public void InvitePlayer()
    {
        if (Offer.State.HasFlag(OfferState.PlayerInvited))
        {
            // re-invite
        }
        else
        {
            // invite
        }

        Offer.State = OfferState.PlayerInvited;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    public void DenyOffer()
    {
        // kick
        Offer.State = OfferState.Done;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    public void Trade()
    {
        // trade
        Offer.State = OfferState.Trading;
        this.RaisePropertyChanged(nameof(BorderBrush));
    }

    #endregion

    #region Private methdos

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