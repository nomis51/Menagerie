using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class ItemModifier : StatMatcher {
        public Stat Stat { get; set; }
        public List<StatMatcher> StatMatchers { get; set; } = new List<StatMatcher>();
        public List<double> Values { get; set; } = new List<double>();
        public ItemModifierType Type { get; set; }
    }
}
