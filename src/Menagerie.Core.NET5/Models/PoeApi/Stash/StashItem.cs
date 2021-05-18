using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Menagerie.Core.Models.PoeApi.Stash {
    public class StashItem {
        [JsonProperty("w")]
        public int Width { get; set; }
        [JsonProperty("h")]
        public int Height { get; set; }
        [JsonProperty("icon")]
        public string IconUrl { get; set; }
        [JsonProperty("typeLine")]
        public string Type { get; set; }
        public string Name { get; set; }
        public bool Identified { get; set; } = true;
        [JsonProperty("ilvl")]
        public int ItemLevel { get; set; }
        public int FrameType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        [JsonProperty("artFilename")]
        public string ArtFileName { get; set; }
    }
}
