using System.Collections.Generic;

namespace Menagerie.Models
{
    public class PriceCheckResult
    {
        public string League { get; set; }
        public List<PricingResult> Results { get; set; }
    }
}