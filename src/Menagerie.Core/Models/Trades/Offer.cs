using System;
using System.Drawing;
using System.Linq;
using LiteDB;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Enums;
using PoeLogsParser.Enums;
using PoeLogsParser.Models;

namespace Menagerie.Core.Models.Trades
{
    public class Offer :  ChatEvent, IDocument
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
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

        public Offer()
        {
            EvenType = ChatEventEnum.Offer;
        }

        public Offer(TradeLogEntry entry)
        {
            ItemName = entry.Item.Name;
            EscapedName = entry.Item.EscapedName;
            PlayerName = entry.Player;
            Time = entry.Time;
            Currency = entry.Price.Currency;
            Price = entry.Price.Value;
            CurrencyImageLink = entry.Price.ImageLink;
            League = entry.League;
            IsOutgoing = entry.Types.Contains(LogEntryType.Outgoing);

            if (entry.Location == null) return;
            StashTab = entry.Location.StashTab;
            Position = new Point(entry.Location.Left, entry.Location.Top);
        }
    }
}