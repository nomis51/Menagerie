using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Trading;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class OutgoingOfferViewModel : ReactiveObject
{
    #region Events

    public delegate void OfferRemovedEvent(OutgoingOfferViewModel vm);

    public event OfferRemovedEvent OnOfferRemoved;

    #endregion

    #region Members

    private bool _hasJoinedHideout;
    private bool _hasSentTradeRequest;
    private string _elapsedTime = string.Empty;

    #endregion

    #region Props

    public OutgoingOfferDto Offer { get; }

    public string ItemNameTrimmed => Offer.ItemName.Length >= 15 ? $"{Offer.ItemName[..15]}..." : Offer.ItemName;
    public string ItemName => Offer.ItemName;

    public string Player => Offer.Player;

    public string League => Offer.League;

    public string Price => Offer.Price.ToString(CultureInfo.InvariantCulture);

    public string PriceStr => Offer.PriceStr;

    public string Time => Offer.Time.ToString("dd MMMM yyyy HH:mm:ss");

    public string PriceConversions => Offer.PriceConversions.Text;

    public bool HasPriceConversions => !string.IsNullOrEmpty(PriceConversions);
    public bool HasImage => Offer.ImageUri is not null;

    public BitmapImage CurrencyImageUri => new(Offer.CurrencyImageUri);
    public BitmapImage? ItemImageUri => HasImage ? new(Offer.ImageUri) : null;

    public bool HasJoinedHideout
    {
        get => _hasJoinedHideout;
        private set => this.RaiseAndSetIfChanged(ref _hasJoinedHideout, value);
    }

    public double PriceFontSize => Offer.Price
          .ToString(CultureInfo.InvariantCulture)
          .Replace(".", "")
          .Length switch
    {
        <= 1 => 24.0d,
        <= 2 => 20.0d,
        <= 3 => 18.0d,
        <=4 => 12.5d,
        <= 5 => 10.5,
        _ => 9.5d
    };

    public bool HasSentTradeRequest
    {
        get => _hasSentTradeRequest;
        private set => this.RaiseAndSetIfChanged(ref _hasSentTradeRequest, value);
    }

    public string ElapsedTime
    {
        get => _elapsedTime;
        private set => this.RaiseAndSetIfChanged(ref _elapsedTime, value);
    }

    #endregion

    #region Constructors

    public OutgoingOfferViewModel(OutgoingOfferDto offer)
    {
        Offer = offer;

        UpdateElapsedTime();
    }

    #endregion

    #region Public methods

    public void SetNextOfferState()
    {
        if (!HasJoinedHideout)
        {
            SendHideoutCommand();
        }
    }

    public void SendTradeRequest()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendTradeRequestCommand(Offer.Player);
    }

    public void SendHideoutCommand()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendHideoutCommand(Offer.Player);
        HasJoinedHideout = true;
    }

    public void RemoveOffer()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendLeavePartyCommand();
        OnOfferRemoved?.Invoke(this);
    }

    public void PrepareToSendWhisper()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.PrepareToSendWhisper(Offer.Player);
    }

    #endregion

    #region Private methods

    private void UpdateElapsedTime()
    {
        Task.Run(() =>
        {
            while (true)
            {
                var seconds = (int)Math.Round((DateTime.Now - Offer.Time).TotalSeconds);
                if (System.Windows.Application.Current is null) return;
                
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ElapsedTime = $"({seconds} second{(seconds > 1 ? "s" : "")} ago)";
                });
                Thread.Sleep(1000);
            }
        });
    }

    #endregion
}