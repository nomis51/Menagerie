using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestQueryFiltersTypeFilters {
        public TypeFilters Filters { get; set; }
    }

    public class TypeFilters {
        public TypeFiltersRarity Rarity { get; set; }
        public TypeFiltersCategory Category { get; set; }
    }

    public class TypeFiltersRarity {
        public string Option { get; set; } = "unique";
    }

    public class TypeFiltersCategory {
        public string Option { get; set; }
    }
}
