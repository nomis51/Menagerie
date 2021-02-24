using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

namespace Menagerie.Core.Models {
    public class Offer : ChatEvent {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string EscapedName {
            get {
                if (string.IsNullOrEmpty(ItemName)) {
                    return "";
                }

                return Escape(ItemName);
            }
        }
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

        private string Escape(string val) {
            return CleanBulkName(CleanMapName(val));
        }

        private string CleanMapName(string val) {
            return Regex.Replace(val, @" \(T[0-9]+\)", "");
        }

        private string CleanBulkName(string val) {
            return Regex.Replace(val, "[0-9]+ ", "");
        }
    }
}
