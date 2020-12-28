using System;
using System.Collections.Generic;
using System.Text;
using Menagerie.Core.Services;

namespace Menagerie.Core.Models {
    public class StatMatcher {
        public string String { get; set; }
        public string Ref { get; set; }
        public bool Negate { get; set; }
        public StatMatcherContition Condition { get; set; }
        public StatMatcherOption Option { get; set; }

        public StatMatcher() { }

        public StatMatcher(StatConditionDto dto) {
            if (dto != null) {
                String = dto.String;
                Ref = dto.Ref;
                Negate = dto.Negate;
                Condition = new StatMatcherContition(dto.Condition);
                Option = new StatMatcherOption(dto.Option);
            }
        }
    }

    public class StatMatcherContition {
        public int Min { get; set; }
        public int Max { get; set; }

        public StatMatcherContition() { }
        public StatMatcherContition(StatConditionConditionDto dto) {
            if (dto != null) {
                Min = dto.Min;
                Max = dto.Max;
            }
        }
    }

    public class StatMatcherOption {
        public string Text { get; set; }
        public string TradeId { get; set; }

        public StatMatcherOption() { }

        public StatMatcherOption(StatConditionOptionDto dto) {
            if (dto != null) {
                Text = dto.Text;
                TradeId = dto.TradeId.ToString();
            }
        }
    }
}
