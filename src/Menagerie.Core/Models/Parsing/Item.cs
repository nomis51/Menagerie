using System.Collections.Generic;
using System.Text.RegularExpressions;
using Menagerie.Core.Enums;

namespace Menagerie.Core.Models.Parsing
{
    public class Item
    {
        public string Name { get; set; }

        public string EscapedName => string.IsNullOrEmpty(Name) ? "" : Escape(Name);

        public int Quantity { get; set; } = 1;
        public ItemRarity Rarity { get; set; }
        public string Type { get; set; }
        public List<ItemRequirement> Requirements { get; set; } = new List<ItemRequirement>();
        public int ItemLevel { get; set; }
        public List<string> Implicits { get; set; } = new List<string>();
        public List<ItemModifier> Modifiers { get; set; } = new List<ItemModifier>();
        public bool IsUnidentified { get; set; } = false;
        public bool IsCorrupted { get; set; } = false;
        public List<ItemInfluence> Influences { get; set; } = new List<ItemInfluence>();
        public string RawText { get; set; }
        public ItemExtra Extra { get; set; } = new ItemExtra();
        public ItemProps Props { get; set; } = new ItemProps();
        public int Quality { get; set; } = 0;
        public ItemStackSize StackSize { get; set; }
        public ItemCategory Category { get; set; } = ItemCategory.None;
        public ItemSockets Sockets { get; set; } = new ItemSockets();
        public ItemHeistJob HeistJob { get; set; } = null;
        public string Icon { get; set; }
        public string ItemType { get; set; }

        private string Escape(string val)
        {
            return CleanBulkName(CleanMapName(val));
        }

        private string CleanMapName(string val)
        {
            return Regex.Replace(val, @" \(T[0-9]+\)", "");
        }

        private string CleanBulkName(string val)
        {
            return Regex.Replace(val, "[0-9]+ ", "");
        }
    }
}