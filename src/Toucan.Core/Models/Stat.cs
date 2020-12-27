using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class Stat {
        public string Text { get; set; }
        public string Ref { get; set; }
        public bool Inverted { get; set; }
        public List<StatType> Types { get; set; } = new List<StatType>();
    }

    public class StatType {
        public string Name { get; set; }
        public string TradeId { get; set; }
    }
}
