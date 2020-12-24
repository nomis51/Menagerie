using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class Offer : ChatEvent {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string PlayerName { get; set; }
        public DateTime Time { get; set; }
        public string Currency { get; set; }
        public double Price { get; set; }
        public string CurrencyImageLink { get; set; }
        public string League { get; set; }
        public bool IsOutgoing { get; set; } = false;

        public Offer() {
            base.EvenType = Core.ChatEvent.Offer;
        }
    }
}
