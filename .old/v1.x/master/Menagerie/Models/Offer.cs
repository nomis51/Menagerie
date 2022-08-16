using System;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using LiteDB;
using Point = System.Drawing.Point;

namespace Menagerie.Models
{
    public class Offer : Screen
    {
        public ObjectId Id { get; set; }
        public string StrId => Id.ToString();
        public string ItemName { get; set; }
        public string EscapedName { get; set; }
        public string PlayerName { get; set; }
        public DateTime Time { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string CurrencyImageLink { get; set; }
        public string League { get; set; }
        public bool IsOutgoing { get; set; } = false;
        public string StashTab { get; set; } = "";
        public Point Position { get; set; }
        public string Notes { get; set; }

        public PriceCheckResult PriceCheck { get; set; }
        public string PriceText => $"{Price}x";
        public OfferState State { get; set; }
        public bool PlayerInvited => State is OfferState.PlayerInvited or OfferState.TradeRequestSent;
        public bool PlayerNotInvited => !PlayerInvited;
        public bool TradeRequestSent => State == OfferState.TradeRequestSent;
        public bool HideoutJoined => State == OfferState.HideoutJoined;
        public bool TradeDone => State == OfferState.Done;
        public bool PlayerJoined { get; set; } = false;
        public bool PossibleScam { get; set; }

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

        public Visibility PossibleScamVisibility => PossibleScam ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsBusyButtonVisibility => PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsReInviteButtonVisibility => PlayerInvited ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsInviteBorderVisibility => PlayerInvited ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsNormalBorderVisibility => PlayerNotInvited ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsTradeBorderVisibility => State == OfferState.TradeRequestSent ? Visibility.Visible : Visibility.Hidden;
        public Visibility PlayerJoinedIconVisibility => PlayerJoined ? Visibility.Visible : Visibility.Hidden;
        public Visibility NotesVisibility => !string.IsNullOrEmpty(Notes) ? Visibility.Visible : Visibility.Hidden;

        public void Notify(string name)
        {
            NotifyOfPropertyChange(name);
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