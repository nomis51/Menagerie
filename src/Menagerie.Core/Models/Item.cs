using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
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
        public string ItemType { get; set; }

        public Item() { }

        public static string ItemCategoryToType(ItemCategory category) {
            switch (category) {
                case ItemCategory.Map:
                    return "Map";

                case ItemCategory.Prophecy:
                    return "Prophecy";

                case ItemCategory.CapturedBeast:
                    return "Beast";

                case ItemCategory.MetamorphSample:
                    return "MetamorphSample";

                case ItemCategory.Seed:
                    return "Seed";

                case ItemCategory.Helmet:
                    return "Armour";

                case ItemCategory.BodyArmour:
                    return "Armour";

                case ItemCategory.Gloves:
                    return "Armour";

                case ItemCategory.Boots:
                    return "Armour";

                case ItemCategory.Shield:
                    return "Armour";

                case ItemCategory.Amulet:
                    return "Accessories";

                case ItemCategory.Belt:
                    return "Accessories";

                case ItemCategory.Ring:
                    return "Accessories";

                case ItemCategory.Flask:
                    return "Flask";

                case ItemCategory.AbyssJewel:
                    return "Jewel";

                case ItemCategory.Jewel:
                    return "Jewel";

                case ItemCategory.Quiver:
                    return "Weapon";

                case ItemCategory.Claw:
                    return "Weapon";

                case ItemCategory.Bow:
                    return "Weapon";

                case ItemCategory.Sceptre:
                    return "Weapon";

                case ItemCategory.Wand:
                    return "Weapon";

                case ItemCategory.FishingRod:
                    return "Weapon";

                case ItemCategory.Staff:
                    return "Weapon";

                case ItemCategory.Warstaff:
                    return "Weapon";

                case ItemCategory.Dagger:
                    return "Weapon";

                case ItemCategory.RuneDagger:
                    return "Weapon";

                case ItemCategory.OneHandedAxe:
                    return "Weapon";

                case ItemCategory.TwoHandedAxe:
                    return "Weapon";

                case ItemCategory.OneHandedMace:
                    return "Weapon";

                case ItemCategory.TwoHandedMace:
                    return "Weapon";

                case ItemCategory.OneHandedSword:
                    return "Weapon";

                case ItemCategory.TwoHandedSword:
                    return "Weapon";

                case ItemCategory.ClusterJewel:
                    return "Jewel";

                case ItemCategory.Watchstone:
                    return "Watchstone";

                case ItemCategory.HeistBlueprint:
                    return "Heist Blueprint";

                case ItemCategory.HeistContract:
                    return "Heist Contract";

                case ItemCategory.HeistTool:
                    return "Heist Tool";

                case ItemCategory.HeistBrooch:
                    return "Heist Brooch";

                case ItemCategory.HeistGear:
                    return "Heist Gear";

                case ItemCategory.HeistCloak:
                    return "Heist Cloak";

                case ItemCategory.Trinket:
                    return "Trinket";

                case ItemCategory.None:
                default:
                    return "";
            }
        }

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
}
