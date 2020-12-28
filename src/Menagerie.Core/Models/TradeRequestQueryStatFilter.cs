using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestQueryStatFilter {
        public string Id { get; set; }
        public StatFilterValue Value { get; set; }
        public bool Disabled { get; set; }
    }

    public class StatFilterValue {
        public double Min { get; set; }
        public double Max { get; set; }
        public string Option { get; set; }
    }
}
