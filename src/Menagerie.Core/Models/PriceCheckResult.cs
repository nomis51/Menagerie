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

        public PricingResult HighestPricing {
            get {
                return Results.OrderBy(r => r.ChaosValue).Last();
            }
        }

        public PricingResult ModePricing {
            get {
                var groups = Results.GroupBy(p => p.ChaosValue);
                int maxCount = groups.Max(r => r.Count());
                var result = groups.First(g => g.Count() == maxCount).First();

                return new PricingResult() {
                    ChaosValue = result.ChaosValue * 0.9d,
                    Currency = "chaos",
                    CurrencyImageLink = AppService.Instance.GetCurrencyImageLink("chaos"),
                    PlayerName = "",
                    Price = result.ChaosValue
                };
            }
        }
    }
}
