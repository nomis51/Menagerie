using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toucan.Core.Services;

namespace Toucan.Core.Models {
    public class Stat {
        public string Text { get; set; }
        public string Ref { get; set; }
        public bool Inverted { get; set; }
        public List<StatType> Types { get; set; } = new List<StatType>();

        public Stat() { }

        public Stat(StatModDto dto) {
            Text = dto.Text;
            Ref = dto.Ref;
            Inverted = dto.Inverted;
            Types = dto.Types.Select(t => new StatType(t)).ToList();
        }
    }

    public class StatType {
        public string Name { get; set; }
        public string TradeId { get; set; }

        public StatType() { }

        public StatType(StatModTypeDto dto) {
            Name = dto.Name;
            TradeId = dto.TradeId[0];
        }
    }
}
