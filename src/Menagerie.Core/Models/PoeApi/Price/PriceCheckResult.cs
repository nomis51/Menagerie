using System.Collections.Generic;
using Menagerie.Core.Models.Parsing;

namespace Menagerie.Core.Models.PoeApi.Price
{
    public class PriceCheckResult
    {
        public Item Item { get; set; }
        public string League { get; set; }
        public List<PricingResult> Results { get; set; }
    }
}