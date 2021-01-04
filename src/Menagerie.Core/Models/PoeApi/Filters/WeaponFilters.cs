using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class WeaponFilters {
        public FilterRange Dps { get; set; }
        [JsonProperty("pdps")]
        public FilterRange PDps { get; set; }
        [JsonProperty("edps")]
        public FilterRange EDps { get; set; }
        public FilterRange Crit { get; set; }
        public FilterRange Aps { get; set; }
    }
}
