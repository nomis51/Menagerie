using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public class FetchResultElementItem
    {
        public string Name { get; set; }
        [JsonProperty("typeLine")] public string Type { get; set; }
        [JsonProperty("ilvl")] public int ILvl { get; set; }
        public int StackSize { get; set; }
        public bool Corrupted { get; set; }
        public string Icon { get; set; }
        public List<FetchResultElementItemProperty> Properties { get; set; }
    }
}