using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class MiscFilters {
        [JsonProperty("ilvl")]
        public FilterRange ILvl { get; set; }
        public FilterRange Quality { get; set; }
        [JsonProperty("gem_levle")]
        public FilterRange GemLevel { get; set; }
        public FilterBoolean Corrupted { get; set; }
        public FilterBoolean Identitified { get; set; }
        [JsonProperty("shaper_item")]
        public FilterBoolean ShaperItem { get; set; }
        [JsonProperty("crusader_item")]
        public FilterBoolean CrusaderItem { get; set; }
        [JsonProperty("hunter_item")]
        public FilterBoolean HunterItem { get; set; }
        [JsonProperty("elder_item")]
        public FilterBoolean ElderItem { get; set; }
        [JsonProperty("redeemer_item")]
        public FilterBoolean RedeemerItem { get; set; }
        [JsonProperty("warlord_item")]
        public FilterBoolean WarlordItem { get; set; }
        [JsonProperty("stack_size")]
        public FilterRange StackSize { get; set; }
        [JsonProperty("gem_alternate_quality")]
        public AlternateQualityGemFilter GemAlternateQuality { get; set; }
    }
}
