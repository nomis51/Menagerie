using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class HeistFilters {
        [JsonProperty("area_level")]
        public FilterRange AreaLevel { get; set; }
        [JsonProperty("heist_agility")]
        public FilterRange HeistAgility { get; set; }
        [JsonProperty("heist_brute_force")]
        public FilterRange HeistBruteForce { get; set; }
        [JsonProperty("heist_counter_thaumaturgy")]
        public FilterRange HeistCounterThaumaturgy { get; set; }
        [JsonProperty("heist_deception")]
        public FilterRange HeistDeception { get; set; }
        [JsonProperty("heist_demolition")]
        public FilterRange HeistDemolition { get; set; }
        [JsonProperty("heist_engineering")]
        public FilterRange HeistEngineering { get; set; }
        [JsonProperty("heist_lockpicking")]
        public FilterRange HeistLockpicking { get; set; }
        [JsonProperty("heist_perception")]
        public FilterRange HeistPerception { get; set; }
        [JsonProperty("heist_trap_disarmament")]
        public FilterRange HeistTrapDisarmament { get; set; }
    }
}
