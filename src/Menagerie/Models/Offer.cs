using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Menagerie.Core.Extensions;

namespace Menagerie.Models
{
    public class Offer : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _itemName;

        public string ItemName
        {
            get => _itemName;
            set
            {
                this._itemName = value;
                this.OnPropertyChanged("ItemName");
            }
        }

        private string _escapedName;

        public string EscapedName
        {
            get => this._escapedName;
            set
            {
                _escapedName = value;
                this.OnPropertyChanged("EscapedName");
            }
        }

        private string _playerName;

        public string PlayerName
        {
            get => _playerName;
            set
            {
                _playerName = value;
                OnPropertyChanged("PlayerName");
            }
        }

        private string _stashTab;

        public string StashTab
        {
            get => _stashTab;
            set
            {
                _stashTab = value;
                OnPropertyChanged("StashTab");
            }
        }

        private System.Drawing.Point _position;

        public System.Drawing.Point Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        private string _notes;

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged("Notes");
            }
        }

        private DateTime _time;

        public DateTime Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        private string _currency;

        public string Currency
        {
            get => _currency;
            set
            {
                _currency = value;
                OnPropertyChanged("Currency");
            }
        }

        private double _price;

        public double Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        private string _currencyImageLink;

        public string CurrencyImageLink
        {
            get => _currencyImageLink;
            set
            {
                _currencyImageLink = value;
                OnPropertyChanged("CurrencyImageLink");
            }
        }

        private string _league;

        public string League
        {
            get => _league;
            set
            {
                _league = value;
                OnPropertyChanged("League");
            }
        }

        public Core.Models.PriceCheckResult PriceCheck { get; set; }
        public bool PossibleScam { get; set; } = false;

        public Visibility PossibleScamVisible => PossibleScam ? Visibility.Visible : Visibility.Hidden;

        public OfferState State { get; set; }

        public Visibility IsBusyButtonVisible => PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsReInviteButtonVisible => PlayerInvited ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsInviteBorderVisible => PlayerInvited ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsNormalBorderVisible => PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsTradeBorderVisible =>
            State == OfferState.TradeRequestSent ? Visibility.Visible : Visibility.Hidden;

        public bool PlayerInvited => State is OfferState.PlayerInvited or OfferState.TradeRequestSent;

        public bool PlayerNotInvited => !PlayerInvited;

        public bool TradeRequestSent => State == OfferState.TradeRequestSent;

        public bool HideoutJoined => State == OfferState.HideoutJoined;

        public bool TradeDone => State == OfferState.Done;

        public bool PlayerJoined { get; set; } = false;

        public string PriceText => $"{Price}x";

        public Visibility PlayerJoinedIconVisible => PlayerJoined ? Visibility.Visible : Visibility.Hidden;

        public double PriceFontSize
        {
            get
            {
                var size = 14.0d;
                var roundedPrice = Math.Round(Price, 1);

                if (roundedPrice < 10.0d && roundedPrice % 1 == 0)
                {
                    size = 20;
                }
                else if ((roundedPrice is >= 10 and < 100) || roundedPrice % 1 != 0)
                {
                    size = 16;
                }

                return size;
            }
        }

        private int MaxNameLength => (IsOutgoing ? 16 : 12);

        public string ItemNameText => ItemName.Length > MaxNameLength ? ItemName[..MaxNameLength] + "..." : ItemName;

        public string PlayerNameText =>
            PlayerName.Length > MaxNameLength ? PlayerName[..MaxNameLength] + "..." : PlayerName;

        public bool IsOutgoing { get; set; }

        public bool IsHighlighted { get; set; } = false;

        public string Tooltip
        {
            get
            {
                var text =
                    $"Time: {Time}\nPlayer: {PlayerName}\nItem: {ItemName}\nPrice: {Price} {Currency}\nLeague: {League}";

                if (!string.IsNullOrEmpty(StashTab))
                {
                    text += $"\nStash tab: {StashTab}\nPosition: Left {Position.X}, Top {Position.Y}";
                }

                if (!string.IsNullOrEmpty(Notes))
                {
                    text += $"\nNotes: {Notes}";
                }

                if (!PossibleScam) return text;
                var prices = PriceCheck.Results.Aggregate("", (current, r) => current + $"{r.Price} {r.Currency}, ");

                prices = prices[..^2];

                text +=
                    $"\n\nPossible scam: You have a {ItemName} listed for\n{prices}\non www.pathofexile.com/trade, but the player is asking for {Price} {Currency}";

                return text;
            }
        }

        public Visibility AnyNotes => !string.IsNullOrEmpty(Notes) ? Visibility.Visible : Visibility.Hidden;

        private List<string> _tradeChatScanWords = new List<string>();

        public string TradeChatScanWords
        {
            get => string.Join(" ", _tradeChatScanWords);
            set
            {
                _tradeChatScanWords = value.Split(' ').ToList();
                OnPropertyChanged("TradeChatScanWords");
            }
        }


        public Offer()
        {
        }

        public Offer(Core.Models.Offer offer)
        {
            this.Id = offer.Id;
            this.ItemName = offer.ItemName;
            this.EscapedName = offer.EscapedName;
            this.PlayerName = offer.PlayerName;
            this.Time = offer.Time;
            this.Currency = offer.Currency;
            this.CurrencyImageLink = offer.CurrencyImageLink;
            this.Price = offer.Price;
            this.League = offer.League;
            this.IsOutgoing = offer.IsOutgoing;
            StashTab = offer.StashTab;
            Position = offer.Position;
            Notes = offer.Notes;
        }

        private string ElapsedTime()
        {
            return (DateTime.Now - Time).ToReadableString();
        }
    }

    public enum OfferState
    {
        Initial,
        PlayerInvited,
        TradeRequestSent,
        Done,
        HideoutJoined
    }
}