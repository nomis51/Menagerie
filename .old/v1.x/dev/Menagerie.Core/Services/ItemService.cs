using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using Menagerie.Core.Models.ItemsScan;

namespace Menagerie.Core.Services
{
    public class ItemService : IService
    {
        #region Members

        //private readonly List<MapModifier> _mapModifiers = new()
        //{
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters' skills Chain 2 additional times", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Skills", "Chain", "Additional"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters fire 2 additional Projectiles", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Projectile", "Additional"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("[0-9]+% increased Monster Damage", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Increased", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters deal [0-9]+% extra Physical Damage as Fire", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Extra", "Fire", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters deal [0-9]+% extra Physical Damage as Cold", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Extra", "Cold", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters deal [0-9]+% extra Physical Damage as Lightning", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Extra", "Fire", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters reflect [0-9]+% of Physical Damage", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Reflect", "Physical", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters reflect [0-9]+% of Elemental Damage", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Reflect", "Elemental", "Damage"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("[0-9]+% less effect of Curses on Monsters", RegexOptions.Compiled),
        //        Keywords = new[] {"Less", "Effect", "Curse"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters cannot be Stunned", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Cannot", "Stun"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters have a [0-9]+% chance to avoid Poison, Blind, and Bleeding",
        //            RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Avoid", "Poison", "Blind", "Bleeding"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters are Hexproof", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Hexproof"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players have Elemental Equilibrium", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Elemental Equilibrium"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players have Point Blank", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Point Blank"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Player chance to Dodge is Unlucky", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Dodge", "Unlucky"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players have [0-9]+% reduced Chance to Block", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Reduce", "Block"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters have [0-9]+% increased Critical Strike Chance", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Increased", "Critical Strike Chance"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players are Cursed with Elemental Weakness, with [0-9]+% increased Effect",
        //            RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Cursed", "Elemental Weakness"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players are Cursed with Vulnerability, with [0-9]+% increased Effect",
        //            RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Cursed", "Vulnerability"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players are Cursed with Temporal Chains, with [0-9]+% increased Effect",
        //            RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Cursed", "Temporal Chains"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Players cannot Regenerate Life, Mana or Energy Shield", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Cannot", "Regeneration", "Life", "Mana", "Energy Shield"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("-[0-9]+% maximum Player Resistances", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Less", "Maximum", "Resistances"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Cannot Leech Life from Monsters", RegexOptions.Compiled),
        //        Keywords = new[] {"Player", "Cannot", "Leech"}
        //    },
        //    new MapModifier()
        //    {
        //        IsBad = true,
        //        Regex = new Regex("Monsters have [0-9]+% chance to Avoid Elemental Ailments", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Avoid", "Elemental Ailments"}
        //    },
        //    // Good mods
        //    new MapModifier()
        //    {
        //        IsGood = true,
        //        Regex = new Regex("Rare Monsters each have a Nemesis Mod", RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Nemesis"}
        //    },
        //    new MapModifier()
        //    {
        //        IsGood = true,
        //        Regex = new Regex("Slaying Enemies close together has a 13% chance to attract monsters from Beyond",
        //            RegexOptions.Compiled),
        //        Keywords = new[] {"Monster", "Beyond", "Additional"}
        //    }
        //};

        #endregion

        #region Constructors

        public ItemService()
        {
        }

        #endregion

        #region Public methods

        public TradeWindowItem ParseTradeWindowItem(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;

            var nameAndType = ParseItemNameAndType(data);

            if (nameAndType == null) return default;

            var stackSize = ParseStackSize(data);
            return new TradeWindowItem()
            {
                Name = nameAndType.Item1,
                Type = nameAndType.Item2,
                StackSize = stackSize
            };
        }

        public int ParseStackSize(string data)
        {
            var startIndex = data.IndexOf("Stack Size:");

            if (startIndex == -1) return 0;

            startIndex += 12;

            var endIndex = data.IndexOf("/", startIndex);

            return endIndex == -1 ? 0 : int.Parse(data.Substring(startIndex, endIndex - startIndex));
        }

        public Tuple<string, string> ParseItemNameAndType(string data)
        {
            if (string.IsNullOrEmpty(data)) return default;

            var dataParts = data.Split("\r\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (dataParts.Length == 0) return default;

            var itemName = "";
            var itemType = "";
            var nbStatsFound = 0;
            var isMap = false;

            for (var i = 0; i < dataParts.Length; ++i)
            {
                if (isMap && dataParts[i].Contains("Map Tier:"))
                {
                    var tier = dataParts[i][dataParts[i].LastIndexOf(" ", StringComparison.Ordinal)..];
                    itemName += $" Tier:{tier}";

                    if (!string.IsNullOrEmpty(itemType))
                    {
                        itemType += $" Tier:{tier}";
                    }

                    break;
                }

                if (dataParts[i].Contains("---"))
                {
                    if (!isMap) break;

                    continue;
                }

                if (dataParts[i].Contains("Rarity:")) continue;

                if (!isMap && dataParts[i].Contains("Item Class:") && dataParts[i].Contains("Map"))
                {
                    isMap = true;
                    continue;
                }

                if (dataParts[i].Contains("Item Class:"))
                {
                    continue;
                }

                switch (nbStatsFound)
                {
                    case 0:
                        itemName = dataParts[i];
                        ++nbStatsFound;
                        break;
                    case 1:
                        itemType = dataParts[i];
                        ++nbStatsFound;
                        break;
                }
            }

            return new Tuple<string, string>(itemName, itemType);
        }

        public List<MapModifier> FindMapModifiers(string data)
        {
            return new List<MapModifier>();
            // return _mapModifiers.FindAll(mod => mod.Regex.IsMatch(data));
        }

        public void Start()
        {
        }

        #endregion
    }
}