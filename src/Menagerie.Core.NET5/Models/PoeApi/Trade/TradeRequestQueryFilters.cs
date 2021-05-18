using Newtonsoft.Json;

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
}
