using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestQueryFilters {
        [JsonProperty("type_filters")]
        public TradeRequestQueryFiltersTypeFilters TypeFilters { get; set; } = null;
        [JsonProperty("socket_filters")]
        public FiltersGroup<SocketFilters> SocketFilters { get; set; }
        [JsonProperty("misc_filters")]
        public FiltersGroup<MiscFilters> MiscFilters { get; set; }
        [JsonProperty("armour_filters")]
        public FiltersGroup<ArmourFilters> ArmourFilters { get; set; }
        [JsonProperty("weapon_filters")]
        public FiltersGroup<WeaponFilters> WeaponFilters { get; set; }
        [JsonProperty("map_filters")]
        public FiltersGroup<MapFilters> MapFilters { get; set; }
        [JsonProperty("heist_filters")]
        public FiltersGroup<HeistFilters> HeistFilters { get; set; }
        [JsonProperty("trade_filters")]
        public FiltersGroup<TradeFilters> TradeFilters { get; set; }
    }

    public class FiltersGroup<T> {
        public T Filters { get; set; }
    }

    public class SocketFilters {
        public FilterRange Links { get; set; }
        public SocketFiltersSockets Sockets { get; set; }
    }

    public class SocketFiltersSockets {
        public int W { get; set; }
    }

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

    public class AlternateQualityGemFilter {
        public int Option { get; set; }
    }

    public class ArmourFilters {
        [JsonProperty("ar")]
        public FilterRange AR { get; set; }
        [JsonProperty("es")]
        public FilterRange ES { get; set; }
        [JsonProperty("ev")]
        public FilterRange EV { get; set; }
        public FilterRange Block { get; set; }
    }

    public class WeaponFilters {
        public FilterRange Dps { get; set; }
        [JsonProperty("pdps")]
        public FilterRange PDps { get; set; }
        [JsonProperty("edps")]
        public FilterRange EDps { get; set; }
        public FilterRange Crit { get; set; }
        public FilterRange Aps { get; set; }
    }

    public class MapFilters {
        [JsonProperty("map_tier")]
        public FilterRange MapTier { get; set; }
        [JsonProperty("map_blighted")]
        public FilterBoolean MapBlighted { get; set; }
    }

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

    public class TradeFilters {
        [JsonProperty("sale_type")]
        public TradeFiltersOption SaleType { get; set; }
        public TradeFiltersOption Indexed { get; set; }
        public FilterRange Price { get; set; }
    }

    public class TradeFiltersOption {
        public string Option { get; set; }
    }
}
