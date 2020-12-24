using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Toucan.Models {
    public class Offer : INotifyPropertyChanged {
        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private int _id;
        public int Id {
            get {
                return _id;
            }
            set {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        private string _itemName;
        public string ItemName {
            get {
                return this._itemName;
            }
            set {
                this._itemName = value;
                this.OnPropertyChanged("ItemName");
            }
        }

        private string _playerName;
        public string PlayerName {
            get {
                return _playerName;
            }
            set {
                _playerName = value;
                OnPropertyChanged("PlayerName");
            }
        }

        private DateTime _time;
        public DateTime Time {
            get {
                return _time;
            }
            set {
                _time = value;
                OnPropertyChanged("Time");
            }
        }

        private string _currency;
        public string Currency {
            get {
                return _currency;
            }
            set {
                _currency = value;
                OnPropertyChanged("Currency");
            }
        }

        private double _price;
        public double Price {
            get {
                return _price;
            }
            set {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        private string _currencyImageLink;
        public string CurrencyImageLink {
            get {
                return _currencyImageLink;
            }
            set {
                _currencyImageLink = value;
                OnPropertyChanged("CurrencyImageLink");
            }
        }

        private string _league;
        public string League {
            get {
                return _league;
            }
            set {
                _league = value;
                OnPropertyChanged("League");
            }
        }

        public OfferState State { get; set; }

        public Visibility IsBusyButtonVisible {
            get {
                return PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility IsReInviteButtonVisible {
            get {
                return PlayerInvited ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility IsInviteBorderVisible {
            get {
                return PlayerInvited ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility IsNormalBorderVisible {
            get {
                return PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility IsTradeBorderVisible {
            get {
                return State == OfferState.TradeRequestSent ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public bool PlayerInvited {
            get {
                return State == OfferState.PlayerInvited || State == OfferState.TradeRequestSent;
            }
        }

        public bool PlayerNotInvited {
            get {
                return !PlayerInvited;
            }
        }

        public bool TradeRequestSent {
            get {
                return State == OfferState.TradeRequestSent;
            }
        }

        public bool HideoutJoined {
            get {
                return State == OfferState.HideoutJoined;
            }
        }

        public bool TradeDone {
            get {
                return State == OfferState.Done;
            }
        }

        public bool PlayerJoined { get; set; } = false;

        public string PriceText {
            get {
                return $"{Price}x";
            }
        }

        public double PriceFontSize {
            get {
                double size = 14.0d;
                double roundedPrice = Math.Round(Price, 1);

                if (roundedPrice < 10.0d && roundedPrice % 1 == 0) {
                    size = 20;
                } else if ((roundedPrice >= 10 && roundedPrice < 100) || roundedPrice % 1 != 0) {
                    size = 16;
                }

                return size;
            }
        }

        private int MaxNameLength {
            get {
                return (IsOutgoing ? 16 : 12);
            }
        }

        public string ItemNameText {
            get {
                return ItemName.Length > MaxNameLength ? ItemName.Substring(0, MaxNameLength) + "..." : ItemName;
            }
        }

        public string PlayerNameText {
            get {
                return PlayerName.Length > MaxNameLength ? PlayerName.Substring(0, MaxNameLength) + "..." : PlayerName;
            }
        }

        public bool IsOutgoing { get; set; }

        public Offer() { }

        public Offer(Core.Models.Offer offer) {
            this.Id = offer.Id;
            this.ItemName = offer.ItemName;
            this.PlayerName = offer.PlayerName;
            this.Time = offer.Time;
            this.Currency = offer.Currency;
            this.CurrencyImageLink = offer.CurrencyImageLink;
            this.Price = offer.Price;
            this.League = offer.League;
            this.IsOutgoing = offer.IsOutgoing;
        }
    }

    public enum OfferState {
        Initial,
        PlayerInvited,
        TradeRequestSent,
        Done,
        HideoutJoined
    }
}
