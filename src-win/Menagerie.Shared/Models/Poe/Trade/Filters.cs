using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Menagerie.Shared.Models.Poe.Trade;

public class Filters
{
    [JsonProperty("trade_filters", NamingStrategyType = typeof(DefaultNamingStrategy))]
    public TradeFilters TradeFilters { get; set; }
}