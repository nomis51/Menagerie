﻿using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Menagerie.Core.Models {
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
        public string StashTab { get; set; } = "";
        public Point Position { get; set; }
        public string Notes { get; set; }

        public Offer() {
            base.EvenType = ChatEventEnum.Offer;
        }
    }
}