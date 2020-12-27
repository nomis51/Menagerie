using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class StatMatcher {
        public string String { get; set; }
        public string Ref { get; set; }
        public bool Negate { get; set; }
        public StatMatcherContition Condition { get; set; }
        public StatMatcherOption Option { get; set; }

        public StatMatcher() { }
    }

    public class StatMatcherContition {
        public int Min { get; set; }
        public int Max { get; set; }

        public StatMatcherContition() { }
    }

    public class StatMatcherOption {
        public string Text { get; set; }
        public string TradeId { get; set; }

        public StatMatcherOption() { }
    }
}
