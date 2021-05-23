using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models.PoeApi.Stash
{
    public class StashTab
    {
        [JsonProperty("n")] public string Name { get; set; }
        [JsonProperty("i")] public int Index { get; set; }
        public string Type { get; set; }
        public bool Hidden { get; set; }
        public bool Selected { get; set; }
        [JsonProperty("colour")] public StashTabColor Color { get; set; }
        public string League { get; set; }
        public List<StashItem> Items { get; set; } = new List<StashItem>();
    }
}