using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public class ArmourFilters
    {
        [JsonProperty("ar")] public FilterRange AR { get; set; }
        [JsonProperty("es")] public FilterRange ES { get; set; }
        [JsonProperty("ev")] public FilterRange EV { get; set; }
        public FilterRange Block { get; set; }
    }
}