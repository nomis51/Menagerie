using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using Menagerie.Application.Services;
using Menagerie.Shared.Models.Trading;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOfferViewModel : ReactiveObject
{
    #region Events

    public delegate void OfferRemovedEvent(IncomingOfferViewModel vm);

    public event OfferRemovedEvent OnOfferRemoved;

    #endregion

    #region Members

    private bool _isPlayerInvited;
    private bool _hasSaidBusy;
    private bool _hasSentTradeRequest;
    private bool _hasPlayerJoined;
    private bool _isScam;
    private string _elapsedTime = string.Empty;
    private string _scamPrice = string.Empty;

    #endregion

    #region Props

    public IncomingOfferDto Offer { get; }

    public string Id => Offer.Id;
    public string StashTab => Offer.StashTab;
    public string Location => $"Left: {Offer.Left}, Top: {Offer.Top}";
    public string ItemNameTrimmed => Offer.ItemName.Length >= 8 ? $"{Offer.ItemName[..8]}..." : Offer.ItemName;
    public string ItemName => Offer.ItemName;
    public string Player => Offer.Player;
    public string League => Offer.League;
    public string Price => Offer.Price.ToString(CultureInfo.InvariantCulture);
    public string PriceStr => Offer.PriceStr;
    public string Time => Offer.Time.ToString("dd MMMM yyyy HH:mm:ss");
    public string PriceConversions => Offer.PriceConversions.Text;
    public bool HasPriceConversions => !string.IsNullOrEmpty(PriceConversions);
    public BitmapImage CurrencyImageUri => new(Offer.CurrencyImageUri);

    public double PriceFontSize
    {
        get
        {
            var value = Offer.Price
                .ToString(CultureInfo.InvariantCulture)
                .Replace(".", "")
                .Length;
            return value switch
                {
                    <= 1 => 20.0d,
                    <= 2 => 18.0d,
                    <= 3 => 15.4d,
                    <= 4 => 10.5d,
                    <= 5 => 8.5,
                    _ => 6.0d
                };
        }
    } 

    public bool IsPlayerInvited
    {
        get => _isPlayerInvited;
        private set => this.RaiseAndSetIfChanged(ref _isPlayerInvited, value);
    }

    public bool HasSaidBusy
    {
        get => _hasSaidBusy;
        private set => this.RaiseAndSetIfChanged(ref _hasSaidBusy, value);
    }

    public bool HasSentTradeRequest
    {
        get => _hasSentTradeRequest;
        private set => this.RaiseAndSetIfChanged(ref _hasSentTradeRequest, value);
    }

    public bool HasPlayerJoined
    {
        get => _hasPlayerJoined;
        private set => this.RaiseAndSetIfChanged(ref _hasPlayerJoined, value);
    }

    public bool IsScam
    {
        get => _isScam;
        private set => this.RaiseAndSetIfChanged(ref _isScam, value);
    }
    
    public string ScamPrice
    {
        get => _scamPrice;
        private set => this.RaiseAndSetIfChanged(ref _scamPrice, value);
    }

    public string ElapsedTime
    {
        get => _elapsedTime;
        private set => this.RaiseAndSetIfChanged(ref _elapsedTime, value);
    }

    #endregion

    #region Constructors

    public IncomingOfferViewModel(IncomingOfferDto offer)
    {
        Offer = offer;
        AppEvents.OnScamDetected += AppEvents_OnScamDetected;

        UpdateElapsedTime();
    }

    private void AppEvents_OnScamDetected(string id, string price)
    {
        if (id != Offer.Id) return;

        System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            ScamPrice = price;
            IsScam = true;
        });
    }

    #endregion

    #region Public methods

    public void SetNextOfferState()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.ToggleItemHighlight(false);

        if (!IsPlayerInvited)
        {
            AppService.Instance.SendInviteCommand(Offer.Player, Offer.ItemName, Offer.PriceStr);
            IsPlayerInvited = true;
            return;
        }

        AppService.Instance.SendTradeRequestCommand(Offer.Player);
        HasSentTradeRequest = true;
    }

    public void SayBusy()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendBusyWhisper(Offer.Player, Offer.ItemName);
        HasSaidBusy = true;
    }

    public void SaySold()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendSoldWhisper(Offer.Player, Offer.ItemName);
        OnOfferRemoved?.Invoke(this);
    }

    public void AskStillInterested()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendStillInterestedWhisper(Offer.Player, Offer.ItemName, PriceStr);
    }

    public void PrepareToSendWhisper()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.PrepareToSendWhisper(Offer.Player);
    }

    public void ReInvite()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendReInvitecommand(Offer.Player);
    }

    public void HighlightItem()
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.SendFindItemInStash(Offer);
    }

    public void Kick()
    {
        AppService.Instance.PlayClickSoundEffect();

        if (IsPlayerInvited)
        {
            AppService.Instance.SendKickCommand(Offer.Player);
        }
        else
        {
            AppService.Instance.EnsureGameFocused();
        }

        OnOfferRemoved?.Invoke(this);
    }

    public void CancelTradeRequest()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate { HasSentTradeRequest = false; });
    }

    public void PlayerHasJoined()
    {
        AppService.Instance.PlayPlayerJoinSoundEffect();
        System.Windows.Application.Current.Dispatcher.Invoke(delegate { HasPlayerJoined = true; });
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
                System.Windows.Application.Current.Dispatcher.Invoke(() => { ElapsedTime = $"({seconds} second{(seconds > 1 ? "s" : "")} ago)"; });
                Thread.Sleep(1000);
            }
        });
    }

    #endregion
}