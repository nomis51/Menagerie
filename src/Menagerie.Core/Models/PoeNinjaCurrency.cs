﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class PoeNinjaCurrency {
        public string CurrencyTypeName { get; set; }
        public PoeNinjaExchange Pay { get; set; }
        public PoeNinjaExchange Receive { get; set; }
        public SparkLine PaySparkLine { get; set; }
        public SparkLine ReceiveSparkLine { get; set; }
        public SparkLine LowConfidencePaySparkLine { get; set; }
        public SparkLine LowConfidenceReviceSparkLine { get; set; }
        public string DetailsId { get; set; }
    }

    public class PoeNinjaExchange {
        public int Id { get; set; }
        [JsonProperty("league_id")]
        public int LeagueId { get; set; }
        [JsonProperty("pay_currency_id")]
        public int PayCurrencyId { get; set; }
        [JsonProperty("get_currency_id")]
        public int GetCurrencyId { get; set; }
        [JsonProperty("sample_time_utc")]
        public DateTime SampleTimeUtc { get; set; }
        public int Count { get; set; }
        public double Value { get; set; }
        [JsonProperty("data_point_count")]
        public int DataPointCount { get; set; }
        [JsonProperty("includes_secondary")]
        public bool IncludesSecondary { get; set; }
    }

    public class SparkLine {
        public double TotalChange { get; set; }
        public List<double?> Data { get; set; }
    }
}
