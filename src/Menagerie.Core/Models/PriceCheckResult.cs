using Menagerie.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menagerie.Core.Models {
    public class PriceCheckResult {
        public Item Item { get; set; }
        public List<PricingResult> Results { get; set; }
        public PricingResult LowestPricing {
            get {
                return Results.OrderBy(r => r.ChaosValue).First();
            }
        }
        public PricingResult AvgPricing {
            get {
                double sum = 0.0d;

                foreach (var r in Results) {
                    sum += r.ChaosValue;
                }

                double avgPrice = sum / (double)Results.Count;

                return new PricingResult() {
                    ChaosValue = avgPrice,
                    Currency = "chaos",
                    CurrencyImageLink = AppService.Instance.GetCurrencyImageLink("chaos"),
                    PlayerName = "",
                    Price = avgPrice
                };
            }
        }
    }
}
