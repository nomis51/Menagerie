using Menagerie.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Menagerie.Core.Models {
    public class PriceCheckResult {
        public string League { get; set; }
        public List<PricingResult> Results { get; set; }
    }
}
