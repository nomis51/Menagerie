using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public class TradeFilters
    {
        [JsonProperty("sale_type")] public TradeFiltersOption SaleType { get; set; }
        public TradeFiltersOption Indexed { get; set; }
        public FilterRange Price { get; set; }
        public TradeFiltersAccount Account { get; set; }
    }
}