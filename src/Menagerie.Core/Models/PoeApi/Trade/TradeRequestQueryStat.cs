using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestQueryStat {
        public string Type { get; set; }
        public FilterRange Value { get; set; }
        public List<TradeRequestQueryStatFilter> Filters { get; set; } = new List<TradeRequestQueryStatFilter>();
        public bool Disabled { get; set; }
    }

    public class FilterRange {
        public double Min { get; set; }
        public double Max { get; set; }
    }

    public class FilterBoolean {
        public bool Option { get; set; }
    }
}
