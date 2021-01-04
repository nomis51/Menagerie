using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class MapFilters {
        [JsonProperty("map_tier")]
        public FilterRange MapTier { get; set; }
        [JsonProperty("map_blighted")]
        public FilterBoolean MapBlighted { get; set; }
    }
}
