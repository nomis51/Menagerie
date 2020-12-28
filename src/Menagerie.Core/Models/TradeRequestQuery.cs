using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestQuery {
        public TradeRequestQueryStatus Status { get; set; } = new TradeRequestQueryStatus();
        public TradeRequestType Name { get; set; } = null;
        public TradeRequestType Type { get; set; } = null;
        public List<TradeRequestQueryStat> Stats { get; set; } = new List<TradeRequestQueryStat>();
        public TradeRequestQueryFilters Filters { get; set; }
    }

    public class TradeRequestQueryStatus {
        public string Option { get; set; } = "online";
    }

    public class TradeRequestType {
        public string Discriminator { get; set; }
        public string Option { get; set; }
    }
}
