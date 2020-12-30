using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class PricingResult {
        public double Price { get; set; }
        public string Currency { get; set; }
        public string CurrencyImageLink { get; set; }
        public string PlayerName { get; set; }
        public double ChaosValue { get; set; }

        public string PricingText {
            get {
                return $"{Price}x";
            }
        }

        public string ChaosValueText {
            get {
                return $"{ChaosValue}x";
            }
        }
    }
}
