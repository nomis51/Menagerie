using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PoeLogsParser.Enums;
using PoeLogsParser.Models;

namespace Menagerie.Core.Models
{
    public class Offer : ChatEvent
    {
        public int Id { get; set; }
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
            base.EvenType = ChatEventEnum.Offer;
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