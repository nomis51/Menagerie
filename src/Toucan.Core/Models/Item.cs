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
        public ItemExtra Extra { get; set; } = new ItemExtra();
        public ItemProps Props { get; set; } = new ItemProps();
        public int Quality { get; set; } = 0;
        public ItemStackSize StackSize { get; set; } 
        public ItemCategory Category { get; set; } = ItemCategory.None;
        public ItemSockets Sockets { get; set; } = new ItemSockets();
        public ItemHeistJob HeistJob { get; set; } = null;
        public string Icon { get; set; }

        public Item() { }

        public static ItemCategory ToItemCategory(string text) {
            switch (text) {
                case "Map":
                    return ItemCategory.Map;

                case "Prophecy":
                    return ItemCategory.Prophecy;

                case "Captured Beast":
                    return ItemCategory.CapturedBeast;

                case "Metamorph Sample":
                    return ItemCategory.MetamorphSample;

                case "Seed":
                    return ItemCategory.Seed;

                case "Helmet":
                    return ItemCategory.Helmet;

                case "Body Armour":
                    return ItemCategory.BodyArmour;

                case "Gloves":
                    return ItemCategory.Gloves;

                case "Boots":
                    return ItemCategory.Boots;

                case "Shield":
                    return ItemCategory.Shield;

                case "Amulet":
                    return ItemCategory.Amulet;

                case "Belt":
                    return ItemCategory.Belt;

                case "Ring":
                    return ItemCategory.Ring;

                case "Flask":
                    return ItemCategory.Flask;

                case "Abyss Jewel":
                    return ItemCategory.AbyssJewel;

                case "Jewel":
                    return ItemCategory.Jewel;

                case "Quiver":
                    return ItemCategory.Quiver;

                case "Claw":
                    return ItemCategory.Claw;

                case "Bow":
                    return ItemCategory.Bow;

                case "Sceptre":
                    return ItemCategory.Sceptre;

                case "Wand":
                    return ItemCategory.Wand;

                case "Fishing Rod":
                    return ItemCategory.FishingRod;

                case "Staff":
                    return ItemCategory.Staff;

                case "Warstaff":
                    return ItemCategory.Warstaff;

                case "Dagger":
                    return ItemCategory.Dagger;

                case "Rune Dagger":
                    return ItemCategory.RuneDagger;

                case "One-Handed Axe":
                    return ItemCategory.OneHandedAxe;

                case "Two-Handed Axe":
                    return ItemCategory.TwoHandedAxe;

                case "One-Handed Mace":
                    return ItemCategory.OneHandedMace;

                case "Two-Handed Mace":
                    return ItemCategory.TwoHandedMace;

                case "One-Handed Sword":
                    return ItemCategory.OneHandedSword;

                case "Two-Handed Sword":
                    return ItemCategory.TwoHandedSword;

                case "Cluster Jewel":
                    return ItemCategory.ClusterJewel;

                case "Watchstone":
                    return ItemCategory.Watchstone;

                case "Heist Blueprint":
                    return ItemCategory.HeistBlueprint;

                case "Heist Contract":
                    return ItemCategory.HeistContract;

                case "Heist Tool":
                    return ItemCategory.HeistTool;

                case "Heist Brooch":
                    return ItemCategory.HeistBrooch;

                case "Heist Gear":
                    return ItemCategory.HeistGear;

                case "Heist Cloak":
                    return ItemCategory.HeistCloak;

                case "Trinket":
                    return ItemCategory.Trinket;

                default:
                    return ItemCategory.None;
            }
        }
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
