using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class Item {
        public ItemRarity Rarity { get; set; } 
        public string Name { get; set; }
        public string Type { get; set; }
        public List<ItemRequirement> Requirements { get; set; } = new List<ItemRequirement>();
        public int ItemLevel { get; set; }
        public List<string> Implicits { get; set; } = new List<string>();
        public List<ItemModifier> Modifiers { get; set; } = new List<ItemModifier>();
        public bool IsUnidentified { get; set; } = false;
        public bool IsCorrupted { get; set; } = false;
        public List<ItemInfluence> Influences { get; set; } = new List<ItemInfluence>();
        public string RawText { get; set; }
        public ItemExtra Extra { get; set; }
        public ItemProps Props { get; set; }
        public int Quality { get; set; } = 0;
        public ItemStackSize StackSize { get; set; }
        public ItemCategory Category { get; set; } = ItemCategory.None;
        public ItemSockets Sockets { get; set; }
        public ItemHeistJob HeistJon { get; set; } = null;
        public string Icon { get; set; }

        public Item() { }
    }

    public enum ItemRarity {
        Normal,
        Magic,
        Rare,
        Unique,
        Gem,
        DivinationCard,
        Currency
    }

    public enum ItemInfluence {
        Crusader,
        Elder,
        Hunter,
        Redeemer,
        Shaper,
        Warlord
    }

    public enum ItemCategory {
        Map,
        Prophecy,
        CapturedBeast,
        MetamorphSample,
        Seed,
        Helmet,
        BodyArmour,
        Gloves,
        Boots,
        Shield,
        Amulet,
        Belt,
        Ring,
        Flask,
        AbyssJewel,
        Jewel,
        Quiver,
        Claw,
        Bow,
        Sceptre,
        Wand,
        FishingRod,
        Staff,
        Warstaff,
        Dagger,
        RuneDagger,
        OneHandedAxe,
        TwoHandedAxe,
        OneHandedMace,
        TwoHandedMace,
        OneHandedSword,
        TwoHandedSword,
        ClusterJewel,
        Watchstone,
        HeistBlueprint,
        HeistContract,
        HeistTool,
        HeistBrooch,
        HeistGear,
        HeistCloak,
        Trinket,
        None
    }

    public enum ItemModifierType {
        Pseudo,
        Explicit,
        Implicit,
        Crafted,
        Enchant,
        None
    }
}
