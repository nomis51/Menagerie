using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Menagerie.Core.Models;
using Menagerie.Core.Services;
using Menagerie.Core;
using Menagerie.Core.Abstractions;

namespace Menagerie.Core {
    public class ParsingService : IService {
        #region Constants
        private const string ITEM_NAME_START_WORD = "to buy your ";
        private const string ITEM_NAME_END_WORD = " listed for ";
        private const string ITEM_NAME_ALTERNATE_END_WORD = " for my ";
        private const string CURRENCY_END_WORD = " in ";
        private const string TRADE_ACCEPTED_MSG = "trade accepted";
        private const string TRADE_CANCELLED_MSG = "trade cancelled";
        private const string PLAYER_NOT_FOUND = "player not in this area";
        private const string PLAYER_JOINED_MSG = " has joined the area";
        private const int MAX_OFFER_LINE_BUFFER = 20;
        private const int MAX_BUFFER_LIFE_MINS = 5;
        private const string SEPARATOR = "--------";
        private const string CANNOT_USER_ITEM_WORD = "You cannot use this item. Its stats will be ignored";
        private const string RARITY_TAG = "Rarity: ";
        private const int SECTION_PARSED = 1;
        private const int SECTION_SKIPPED = 0;
        private const int PARSER_SKIPPED = -1;
        private readonly List<List<List<int>>> PLACEHOLDER_MAP = new List<List<List<int>>>() {
            // 0 #
            new List<List<int>>() {
                new List<int>()
            },
            // 1 #
            new List<List<int>>() {
                new List<int>{0},
                new List<int>()
            },
            // 2 #
            new List<List<int>>() {
                new List<int>(){0,1},
                new List<int>{0},
                new List<int>(){1},
                new List<int>()
            },
            // 3 #
            new List<List<int>>() {
                new List<int>(){0,1,2},
                new List<int>(){1,2},
                new List<int>(){0,2},
                new List<int>(){0,1},
                new List<int>(){2},
                new List<int>(){1},
                new List<int>(){0}
            },
            // 4 #
            new List<List<int>>() {
                new List<int>(){0,1,2,3},
                new List<int>(){1,2,3},
                new List<int>(){0,2,3},
                new List<int>(){0,1,3},
                new List<int>(){0,1,2},
                new List<int>(){2,3},
                new List<int>(){1,3},
                new List<int>(){1,2},
                new List<int>(){0,3},
                new List<int>(){0,2},
                new List<int>(){0,1}
            }
        };
        #endregion

        #region Members
        private List<Func<List<string>, Item, int>> ItemParsers = new List<Func<List<string>, Item, int>>();
        private List<string> LastOffersLines = new List<string>();
        private List<DateTime> LastOffersTimes = new List<DateTime>();
        private static int Id = 0;
        private HashSet<string> BaseTypes = null;
        #endregion

        #region Constructors
        public ParsingService() {
           
        }
        #endregion

        #region Private methods
        private async void DoCleanBuffer() {
            while (true) {
                await Task.Delay(MAX_BUFFER_LIFE_MINS * 1000 * 60);
                CleanBuffer();

            }
        }

        private void OnNewChatEventParsed(Models.ChatEvent evt) {
            if (evt == null) {
                return;
            }

            switch (evt.EvenType) {
                case Enums.ChatEventEnum.Offer:
                    AppService.Instance.NewOffer((Offer)evt);
                    break;

                case Enums.ChatEventEnum.PlayerJoined:
                    AppService.Instance.NewPlayerJoined(((JoinEvent)evt).PlayerName);
                    break;

                default:
                    AppService.Instance.NewChatEvent(evt.EvenType);
                    break;
            }
        }

        private Models.ChatEvent ParseLine(string aline, bool isClientFileLine = true) {
            Models.ChatEvent evt = null;
            bool processed = false;

            foreach (var line in new List<string> { aline }) {
                Offer offer = new Offer();

                // Time
                if (isClientFileLine) {
                    int timeIndex = line.IndexOf(" ");

                    if (timeIndex == -1) {
                        continue;
                    }

                    timeIndex = line.IndexOf(" ", timeIndex + 1);

                    if (timeIndex == -1) {
                        continue;
                    }

                    var strTime = line.Substring(0, timeIndex);

                    if (string.IsNullOrEmpty(strTime) || string.IsNullOrWhiteSpace(strTime)) {
                        continue;
                    }

                    DateTime date;

                    if (!DateTime.TryParse(strTime.Replace("/", "-"), out date)) {
                        continue;
                    }

                    offer.Time = date;
                } else {
                    offer.Time = DateTime.Now;
                }

                // Player name
                int playerStartIndex = line.IndexOf("@From ");

                if (playerStartIndex == -1) {
                    playerStartIndex = line.IndexOf("@To ");

                    if (playerStartIndex == -1) {
                        // Not a whisper
                        continue;
                    }

                    offer.IsOutgoing = true;
                    playerStartIndex += 4;
                } else {
                    playerStartIndex += 6;
                }

                int playerEndIndex = -1;

                playerEndIndex = line.IndexOf(": ", playerStartIndex);

                if (playerEndIndex == -1) {
                    continue;
                }

                offer.PlayerName = line.Substring(playerStartIndex, playerEndIndex - playerStartIndex);

                if (offer.PlayerName.IndexOf("<") != -1) {
                    int nameIndex = offer.PlayerName.IndexOf(">");

                    if (nameIndex == -1) {
                        continue;
                    }

                    offer.PlayerName = offer.PlayerName.Substring(nameIndex + 2);
                }

                // Item name
                int itemNameStartIndex = line.IndexOf(ITEM_NAME_START_WORD);

                if (itemNameStartIndex == -1) {
                    continue;
                }

                itemNameStartIndex += ITEM_NAME_START_WORD.Length;

                int itemNameEndIndex = line.IndexOf(ITEM_NAME_END_WORD, itemNameStartIndex);

                bool alternateItemNameEndWord = false;

                if (itemNameEndIndex == -1) {
                    itemNameEndIndex = line.IndexOf(ITEM_NAME_ALTERNATE_END_WORD, itemNameStartIndex);
                    alternateItemNameEndWord = true;

                    if (itemNameEndIndex == -1) {
                        continue;
                    }
                }

                offer.ItemName = line.Substring(itemNameStartIndex, itemNameEndIndex - itemNameStartIndex);

                // Price
                int priceStartIndex = itemNameEndIndex + (alternateItemNameEndWord ? ITEM_NAME_ALTERNATE_END_WORD : ITEM_NAME_END_WORD).Length;
                int priceEndIndex = line.IndexOf(" ", priceStartIndex);

                if (priceEndIndex == -1) {
                    continue;
                }
                var value = line.Substring(priceStartIndex, priceEndIndex - priceStartIndex)
                    .Trim()
                    .Replace(".", ",");

                offer.Price = Convert.ToDouble(value);

                // Currency
                int currencyStartIndex = priceEndIndex + 1;
                int currencyEndIndex = line.IndexOf(CURRENCY_END_WORD, currencyStartIndex);

                if (currencyEndIndex == -1) {
                    continue;
                }

                offer.Currency = line.Substring(currencyStartIndex, currencyEndIndex - currencyStartIndex);
                offer.CurrencyImageLink = AppService.Instance.GetCurrencyImageLink(offer.Currency);

                // League
                int leagueStartIndex = currencyEndIndex + CURRENCY_END_WORD.Length;
                int leagueEndIndex = line.IndexOf(" ", leagueStartIndex);

                if (leagueEndIndex == -1) {
                    offer.League = line.Substring(leagueStartIndex);
                } else {
                    offer.League = line.Substring(leagueStartIndex, leagueEndIndex - leagueStartIndex);
                }

                offer.Id = NextId();

                evt = offer;
                processed = true;

                ToBuffer(line);
            }

            if (!processed) {
                var line = aline.ToLower();

                if (line.IndexOf(TRADE_ACCEPTED_MSG) != -1) {
                    evt = new Models.ChatEvent() { EvenType = Enums.ChatEventEnum.TradeAccepted };
                } else if (line.IndexOf(TRADE_CANCELLED_MSG) != -1) {
                    evt = new Models.ChatEvent() { EvenType = Enums.ChatEventEnum.TradeCancelled };
                } else if (line.IndexOf(PLAYER_JOINED_MSG) != -1) {
                    var startIndex = aline.IndexOf("] ");
                    var endIndex = aline.IndexOf(PLAYER_JOINED_MSG);

                    if (startIndex != -1 && endIndex != -1) {
                        evt = new JoinEvent(aline.Substring(startIndex + 2, endIndex - startIndex - 2));
                    }
                }
            }

            return evt;
        }

        private int NextId() {
            return ++Id;
        }

        private void CleanBuffer() {
            for (int i = 0; i < LastOffersTimes.Count; ++i) {
                if ((DateTime.Now - LastOffersTimes.ElementAt(i)).TotalMinutes >= MAX_BUFFER_LIFE_MINS) {
                    LastOffersLines.RemoveAt(i);
                    LastOffersTimes.RemoveAt(i);
                    --i;
                }
            }
        }

        private void ToBuffer(string line) {
            LastOffersLines.Add(line);
            LastOffersTimes.Add(DateTime.Now);

            if (LastOffersLines.Count > MAX_OFFER_LINE_BUFFER) {
                LastOffersLines.RemoveAt(0);
                LastOffersTimes.RemoveAt(0);
            }
        }

        private ItemRarity StringToRarity(string text) {
            switch (text) {
                case "Unique":
                    return ItemRarity.Unique;

                case "Rare":
                    return ItemRarity.Rare;

                case "Magic":
                    return ItemRarity.Magic;

                case "Normal":
                default:
                    return ItemRarity.Normal;
            }
        }

        private Item ParseItemSection(List<string> section) {
            if (!section[0].StartsWith(RARITY_TAG)) {
                return null;
            }

            string rarityText = section[0].Substring(RARITY_TAG.Length);
            ItemRarity rarity;

            switch (rarityText) {
                case "Currency":
                    rarity = ItemRarity.Currency;
                    break;

                case "Divination Card":
                    rarity = ItemRarity.DivinationCard;
                    break;

                case "Gem":
                    rarity = ItemRarity.Gem;
                    break;

                case "Normal":
                    rarity = ItemRarity.Normal;
                    break;

                case "Magic":
                    rarity = ItemRarity.Magic;
                    break;

                case "Rare":
                    rarity = ItemRarity.Rare;
                    break;

                case "Unique":
                    rarity = ItemRarity.Unique;
                    break;

                default:
                    return null;
            }

            return new Item() {
                Rarity = rarity,
                Name = Regex.Replace(section[1], "^(<<.*?>>|<.*?>)+", ""),
                Type = section.Count > 2 ? Regex.Replace(section[2], "^(<<.*?>>|<.*?>)+", "") : null,
            };
        }

        private void SetupItemParsers() {
            ItemParsers.Add(ParseUnidentified);
            ItemParsers.Add(ParseSynthesised);
            ItemParsers.Add(ParseCatergoryByHelpText);
            ItemParsers.Add(NormalizeName);
            // ---
            ItemParsers.Add(ParseItemLevel);
            ItemParsers.Add(ParseVaalGem);
            ItemParsers.Add(ParseGem);
            ItemParsers.Add(ParseArmour);
            ItemParsers.Add(ParseWeapon);
            ItemParsers.Add(ParseFlask);
            ItemParsers.Add(ParseStackSize);
            ItemParsers.Add(ParseCorrupted);
            ItemParsers.Add(ParseInfluence);
            ItemParsers.Add(ParseMap);
            ItemParsers.Add(ParseSockets);
            ItemParsers.Add(ParseProphecyMaster);
            ItemParsers.Add(ParseHeistMission);
            ItemParsers.Add(ParseModifiers);
            ItemParsers.Add(ParseModifiers);
            ItemParsers.Add(ParseModifiers);
        }

        private int ParseMap(List<string> section, Item item) {
            if (section[0].StartsWith("Map Tier: ")) {
                item.Props.MapTier = int.Parse(section[0].Substring("Map Tier: ".Length));

                var name = string.IsNullOrEmpty(item.Type) ? item.Name : item.Type;

                if (name.IndexOf("Blighted ") != -1) {
                    name = name.Substring("Blighted ".Length);

                    if (!string.IsNullOrEmpty(item.Type)) {
                        item.Type = name;
                    } else {
                        item.Name = name;
                    }

                    item.Category = ItemCategory.Map;
                    item.Props.BlightedMap = true;
                }

                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseFlask(List<string> section, Item item) {
            foreach (var line in section) {
                if (Regex.IsMatch(line, @"Current;y has \d+ Charges")) {
                    return SECTION_PARSED;
                }
            }

            return SECTION_SKIPPED;
        }

        private string MagicBaseType(string name) {
            if (BaseTypes == null) {
                BaseTypes = new HashSet<string>();

                var baseTypes = AppService.Instance.GetBaseTypes();

                foreach (var tuple in baseTypes) {
                    BaseTypes.Add(tuple.Item1);

                    if (tuple.Item2.Category == ItemCategory.Map) {
                        BaseTypes.Add($"Blighted {tuple.Item1}");
                    }
                }
            }

            var words = name.Split(' ');

            var perm = words.SelectMany((k, start) => {
                int idx = 0;

                List<string> r = new List<string>(words.Length - start)
                    .Select(v => {
                        string g = string.Join(" ", words.Skip(start).Take(start + idx + 1));
                        ++idx;
                        return g;
                    })
                        .ToList();

                return r;
            });

            var result = perm.Select(n => new Tuple<string, bool>(n, BaseTypes.Contains(n)))
                .ToList()
                .FindAll(r => r.Item2)
                .OrderBy(r => r.Item1)
                .ToList();

            return result.Count > 0 ? result[0].Item1 : null;
        }

        private int NormalizeName(List<string> section, Item item) {
            if (item.Rarity == ItemRarity.Normal || (item.Rarity == ItemRarity.Magic && item.IsUnidentified) || (item.Rarity == ItemRarity.Rare && item.IsUnidentified) || (item.Rarity == ItemRarity.Unique && item.IsUnidentified)) {
                if (item.Name.IndexOf("Superior ") != -1) {
                    item.Name = item.Name.Substring("Superior ".Length);
                }
            }

            if (item.Rarity == ItemRarity.Magic) {
                var baseType = MagicBaseType(item.Name);

                if (!string.IsNullOrEmpty(baseType)) {
                    item.Name = baseType;
                }
            }

            if (item.Category == ItemCategory.MetamorphSample) {
                if (item.Name.IndexOf(" Brain") != -1) {
                    item.Name = "Metamorph Brain";
                } else if (item.Name.IndexOf(" Eye") != -1) {
                    item.Name = "Metamorph Eye";
                } else if (item.Name.IndexOf(" Lung") != -1) {
                    item.Name = "Metamorph Lung";
                } else if (item.Name.IndexOf(" Heart") != -1) {
                    item.Name = "Metamorph Heart";
                } else if (item.Name.IndexOf(" Liver") != -1) {
                    item.Name = "Metamorph Liver";
                }
            }

            if (item.Category == ItemCategory.None) {
                var baseTypes = AppService.Instance.GetBaseTypes();
                var baseType = baseTypes.FirstOrDefault(t => t.Item1 == item.Type);

                if (baseType == null) {
                    baseType = baseTypes.FirstOrDefault(t => t.Item1 == item.Name);
                }

                if (baseType != null) {
                    item.Category = baseType.Item2.Category;
                    item.Icon = baseType.Item2.Icon;
                }
            }

            return PARSER_SKIPPED;
        }

        private int ParseInfluence(List<string> section, Item item) {
            if (section.Count <= 2) {
                var countBefore = item.Influences.Count;

                foreach (var line in section) {
                    switch (line) {
                        case "Crusader Item":
                            item.Influences.Add(ItemInfluence.Crusader);
                            break;

                        case "Elder Item":
                            item.Influences.Add(ItemInfluence.Elder);
                            break;

                        case "Shaper Item":
                            item.Influences.Add(ItemInfluence.Shaper);
                            break;

                        case "Hunter Item":
                            item.Influences.Add(ItemInfluence.Hunter);
                            break;

                        case "Redeemer Item":
                            item.Influences.Add(ItemInfluence.Redeemer);
                            break;

                        case "Warlord Item":
                            item.Influences.Add(ItemInfluence.Warlord);
                            break;
                    }
                }

                if (countBefore < item.Influences.Count) {
                    return SECTION_PARSED;
                }
            }

            return SECTION_SKIPPED;
        }

        private int ParseCorrupted(List<string> section, Item item) {
            if (section[0] == "Corrupted") {
                item.IsCorrupted = true;
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseUnidentified(List<string> section, Item item) {
            if (section[0] == "Unidentified") {
                item.IsUnidentified = true;
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseItemLevel(List<string> section, Item item) {
            if (section[0].StartsWith("Item Level: ")) {
                item.ItemLevel = int.Parse(section[0].Substring("Item Level: ".Length));
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseVaalGem(List<string> section, Item item) {
            if (item.Rarity != ItemRarity.Gem) {
                return PARSER_SKIPPED;
            }

            if (section.Count == 1) {
                if (section[0].IndexOf("Anomalous") != -1) {
                    item.Extra.AltQuality = ItemAltQuality.Anomalous;
                } else if (section[0].IndexOf("Divergent") != -1) {
                    item.Extra.AltQuality = ItemAltQuality.Divergent;
                } else if (section[0].IndexOf("Phantasmal") != -1) {
                    item.Extra.AltQuality = ItemAltQuality.Phantasmal;
                } else if (section[0].IndexOf("Vaal") != -1) {
                    item.Extra.AltQuality = ItemAltQuality.Superior;
                }

                string gemName = Regex.Replace(section[0], "(Anomalous |Divergent |Phantasmal )", "");

                if (!string.IsNullOrEmpty(gemName)) {
                    item.Name = gemName;
                    return SECTION_PARSED;
                }
            }
            return SECTION_SKIPPED;
        }

        private int ParseGem(List<string> section, Item item) {
            if (item.Rarity != ItemRarity.Gem) {
                return PARSER_SKIPPED;
            }

            if (section.Count > 1 && section[1].StartsWith("Level: ")) {
                item.Props.GemLevel = int.Parse(section[1].Substring("Level: ".Length, 10));

                ParseQualityNested(section, item);

                if (item.Extra.AltQuality != ItemAltQuality.None) {
                    if (section[0].IndexOf("Anomalous") != -1) {
                        item.Extra.AltQuality = ItemAltQuality.Anomalous;
                    } else if (section[0].IndexOf("Divergent") != -1) {
                        item.Extra.AltQuality = ItemAltQuality.Divergent;
                    } else if (section[0].IndexOf("Phantasmal") != -1) {
                        item.Extra.AltQuality = ItemAltQuality.Phantasmal;
                    } else if (section[0].IndexOf("Vaal") != -1) {
                        item.Extra.AltQuality = ItemAltQuality.Superior;
                    }

                    string gemName = Regex.Replace(section[0], "(Anomalous |Divergent |Phantasmal )", "");


                    if (!string.IsNullOrEmpty(gemName)) {
                        item.Name = gemName;
                    }
                }

                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseQualityNested(List<string> section, Item item) {
            foreach (var line in section) {
                if (line.StartsWith("Quality: ")) {
                    item.Quality = int.Parse(string.Join("", line.Substring("Quality: ".Length, 10).Where(c => Char.IsDigit(c))));
                    return SECTION_PARSED;
                }
            }

            return SECTION_SKIPPED;
        }

        private int ParseStackSize(List<string> section, Item item) {
            if (item.Rarity != ItemRarity.Currency && item.Rarity != ItemRarity.DivinationCard) {
                return PARSER_SKIPPED;
            }

            if (section[0].StartsWith("Stack Size: ")) {
                var stack = Regex.Replace(section[0].Substring("Stack Size: ".Length), @"[^\d/]", "").Split('/');
                item.StackSize.Value = int.Parse(stack[0]);
                item.StackSize.Max = int.Parse(stack[1]);

                if (item.Category == ItemCategory.Seed) {
                    ParseSeedLevelNested(section, item);
                }

                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseSeedLevelNested(List<string> section, Item item) {
            foreach (var line in section) {
                int startIndex = line.IndexOf("Spawns a Level ");

                if (startIndex == -1) {
                    return SECTION_SKIPPED;
                }

                startIndex += "Spawns a Level ".Length;

                int endIndex = line.IndexOf(" Monster when Harvested", startIndex);

                if (endIndex == -1) {
                    return SECTION_SKIPPED;
                }

                item.ItemLevel = int.Parse(line.Substring(startIndex, endIndex - startIndex));

                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseSockets(List<string> section, Item item) {
            if (section[0].StartsWith("Sockets: ")) {
                string sockets = section[0].Substring("Sockets: ".Length).TrimEnd();

                item.Sockets.NbWhite = sockets.Split('W').Length - 1;
                item.Sockets.NbLinked = sockets == "#-#-#-#-#-#" ?
                    6 :
                    new List<string> { "# #-#-#-#-#", "#-#-#-#-# #", "#-#-#-#-#" }.IndexOf(sockets) != -1 ? 5 : 0;

                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseArmour(List<string> section, Item item) {
            var isParsed = SECTION_SKIPPED;

            foreach (var line in section) {
                if (line.StartsWith("Armour: ")) {
                    item.Props.Armour = int.Parse(string.Join("", line.Substring("Armour: ".Length, 10).Where(c => Char.IsDigit(c))));
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Evasion Rating: ")) {
                    item.Props.Evasion = int.Parse(string.Join("", line.Substring("Evasion Rating: ".Length, 10).Where(c => Char.IsDigit(c))));
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Energy Shield: ")) {
                    item.Props.EnergyShield = int.Parse(string.Join("", line.Substring("Energy Shield: ".Length, 10).Where(c => Char.IsDigit(c))));
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Chance to Block: ")) {
                    item.Props.BlockChance = int.Parse(string.Join("", line.Substring("Chance to Block: ".Length, 10).Where(c => Char.IsDigit(c))));
                    isParsed = SECTION_PARSED;
                    continue;
                }
            }

            if (isParsed == SECTION_PARSED) {
                ParseQualityNested(section, item);
            }

            return isParsed;
        }

        private int RollCountDecimals(double value) {
            return value % 1 == 0 ? 0 : 2;
        }

        private double GetRollAsSingleNumber(double[] values) {
            if (values.Length == 1) {
                return values[0];
            } else {
                var avg = (values[0] + values[1]) / 2;

                var maxPrecision = Math.Max(RollCountDecimals(values[0]), RollCountDecimals(values[1]));
                var rounding = Math.Pow(10, maxPrecision);
                return Math.Floor((avg + Math.Pow(2, -52)) * rounding) / rounding;
            }
        }

        private int ParseWeapon(List<string> section, Item item) {
            var isParsed = SECTION_SKIPPED;

            foreach (var line in section) {
                if (line.StartsWith("Critical Strike Chance: ")) {
                    item.Props.CritChance = double.Parse(line.Substring("Critical Strike Chance: ".Length));
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Attacks per Second: ")) {
                    item.Props.AttackSpeed = double.Parse(line.Substring("Attacks per Second: ".Length));
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Physical Damage: ")) {
                    item.Props.PhysicalDamage = line.Substring("Physical Damage: ".Length)
                        .Split('-')
                        .Select(s => int.Parse(s))
                        .ToList();
                    isParsed = SECTION_PARSED;
                    continue;
                }

                if (line.StartsWith("Elemental Damage: ")) {
                    item.Props.ElementalDamage = line.Substring("Elemental Damage: ".Length)
                        .Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => GetRollAsSingleNumber(
                            s.Split('-')
                            .Select(e => double.Parse(e))
                            .ToArray()
                        ))
                        .Aggregate(0.0d, (sum, x) => sum + x);
                    isParsed = SECTION_PARSED;
                    continue;
                }
            }

            if (isParsed == SECTION_PARSED) {
                ParseQualityNested(section, item);
            }

            return isParsed;
        }

        private string ItemModifierTypeToString(ItemModifierType type) {
            switch (type) {
                case ItemModifierType.Crafted:
                    return "crafted";
                case ItemModifierType.Enchant:
                    return "enchant";
                case ItemModifierType.Explicit:
                    return "explicit";
                case ItemModifierType.Implicit:
                    return "implicit";
                case ItemModifierType.Pseudo:
                    return "pseudo";
                default:
                    return "none";
            }
        }

        private int ParseModifiers(List<string> section, Item item) {
            if (new List<ItemRarity>() { ItemRarity.Normal, ItemRarity.Magic, ItemRarity.Rare, ItemRarity.Unique }.IndexOf(item.Rarity) == -1) {
                return PARSER_SKIPPED;
            }

            var countBefore = item.Modifiers.Count;
            var statIterator = SectionToStatString(section).ToList();

            for (int i = 0; i < statIterator.Count; ++i) {
                var stat = statIterator[i];

                if (ParseVeiledNested(stat, item)) {
                    continue;
                }

                ItemModifierType modType = ItemModifierType.None;

                if (stat.EndsWith(" (implicit)")) {
                    stat = stat.Substring(0, stat.Length - " (implicit)".Length);
                    modType = ItemModifierType.Implicit;
                } else if (stat.EndsWith(" (crafted)")) {
                    stat = stat.Substring(0, stat.Length - " (crafted)".Length);
                    modType = ItemModifierType.Crafted;
                } else if (stat.EndsWith(" (enchant)")) {
                    stat = stat.Substring(0, stat.Length - " (enchant)".Length);
                    modType = ItemModifierType.Enchant;
                }

                var mod = TryFindModifier(stat);

                if (mod != null) {
                    if (modType == ItemModifierType.None) {
                        var isExplicit = mod.Stat.Types.FirstOrDefault(s => s.Name == "explicit");

                        if (isExplicit != null) {
                            modType = ItemModifierType.Explicit;
                        }
                    }

                    if (mod.Stat.Types.FirstOrDefault(s => s.Name == ItemModifierTypeToString(modType)) != null) {
                        mod.Type = modType;
                        item.Modifiers.Add(mod);
                    }
                }
            }

            if (countBefore < item.Modifiers.Count) {
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private bool ParseVeiledNested(string text, Item item) {
            if (text == "Veiled Suffix") {
                item.Extra.Veiled = item.Extra.Veiled == ItemVeiled.None ? ItemVeiled.Suffix : ItemVeiled.PrefixAndSuffix;
                return true;
            }

            if (text == "Veiled Prefix") {
                item.Extra.Veiled = item.Extra.Veiled == ItemVeiled.None ? ItemVeiled.Prefix : ItemVeiled.PrefixAndSuffix;
                return true;
            }

            return false;
        }

        private int ParseSynthesised(List<string> section, Item item) {
            if (section.Count == 1) {
                if (section[0] == "Synthesised Item") {
                    if (!string.IsNullOrEmpty(item.Type)) {
                        item.Type = item.Type.Substring("Synthesised Item".Length);
                    } else {
                        item.Name = item.Name.Substring("Synthesised Item".Length);
                    }

                    return SECTION_PARSED;
                }
            }

            return SECTION_SKIPPED;
        }

        private int ParseCatergoryByHelpText(List<string> section, Item item) {
            if (section[0] == "Right-click to add this prophecy to your character.") {
                item.Category = ItemCategory.Prophecy;
                return SECTION_PARSED;
            } else if (section[0] == "Right-click to add this to your bestiary.") {
                item.Category = ItemCategory.CapturedBeast;
                return SECTION_PARSED;
            } else if (section[0] == "Combine this with four other different samples in Tane's Laboratory.") {
                item.Category = ItemCategory.MetamorphSample;
                return SECTION_PARSED;
            } else if (section[0].StartsWith("Right-click this item then left-click the ground to plant it in the Sacred Grove.")) {
                item.Category = ItemCategory.Seed;
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseProphecyMaster(List<string> section, Item item) {
            if (item.Category != ItemCategory.Prophecy) {
                return PARSER_SKIPPED;
            }

            if (section[0] == "You will find Alva and complete her mission.") {
                item.Extra.ProphecyMaster = ProphecyMaster.Alva;
                return SECTION_PARSED;
            } else if (section[0] == "You will find Einhar and complete his mission.") {
                item.Extra.ProphecyMaster = ProphecyMaster.Einhar;
                return SECTION_PARSED;
            } else if (section[0] == "You will find Niko and complete his mission.") {
                item.Extra.ProphecyMaster = ProphecyMaster.Niko;
                return SECTION_PARSED;
            } else if (section[0] == "You will find Jun and complete her mission.") {
                item.Extra.ProphecyMaster = ProphecyMaster.Jun;
                return SECTION_PARSED;
            } else if (section[0] == "You will find Zana and complete her mission.") {
                item.Extra.ProphecyMaster = ProphecyMaster.Zana;
                return SECTION_PARSED;
            }

            return SECTION_SKIPPED;
        }

        private int ParseHeistMission(List<string> section, Item item) {
            if (item.Category != ItemCategory.HeistBlueprint && item.Category != ItemCategory.HeistContract) {
                return PARSER_SKIPPED;
            }

            foreach (var line in section) {
                if (line.StartsWith("Area Level: ")) {
                    item.Props.AreaLevel = int.Parse(line.Substring("Area Level: ".Length));
                    break;
                }
            }

            if (item.Props.AreaLevel == 0) {
                return SECTION_SKIPPED;
            }

            if (item.Category == ItemCategory.HeistContract) {
                Match match = null;

                foreach (var line in section) {
                    if ((match = Regex.Match(line, @"Requires (.+) \(Level (\d+)\)")).Success) {
                        break;
                    }
                }

                if (match == null || !match.Success) {
                    throw new Exception("never");
                }

                item.HeistJob = new ItemHeistJob() {
                    Name = ToHeistJob(match.Groups[0].Value),
                    Level = int.Parse(match.Groups[1].Value)
                };
            }

            return SECTION_PARSED;
        }

        private HeistJob ToHeistJob(string text) {
            switch (text) {
                case "Lockpicking":
                    return HeistJob.Lockpicking;
                case "Counter-Thaumaturgy":
                    return HeistJob.Counter_Thaumaturgy;
                case "Perception":
                    return HeistJob.Perception;
                case "Deception":
                    return HeistJob.Deception;
                case "Agility":
                    return HeistJob.Agility;
                case "Engineering":
                    return HeistJob.Engineering;
                case "Trap Disarmament":
                    return HeistJob.TrapDisarmament;
                case "Demolition":
                    return HeistJob.Demolition;
                case "Brute Force":
                    return HeistJob.BruteForce;
                default:
                    return HeistJob.Lockpicking;
            }
        }

        private ItemModifier TryFindModifier(string stat) {
            List<string> matches = new List<string>();
            string withPlaceholders = stat;
            withPlaceholders = Regex.Replace(withPlaceholders, @"(?<![\d#])[+-]?[\d.]+", new MatchEvaluator(a => {
                matches.Add(a.Value);
                return "#";
            }), RegexOptions.Multiline);

            if (matches.Count >= PLACEHOLDER_MAP.Count) {
                return null;
            }

            var comboVariants = PLACEHOLDER_MAP[matches.Count];

            foreach (var combo in comboVariants) {
                var pIdx = -1;
                var possibleStat = "";
                possibleStat = Regex.Replace(withPlaceholders, "#", new MatchEvaluator(a => {
                    ++pIdx;
                    if (combo.IndexOf(pIdx) != -1) {
                        return matches[pIdx];
                    } else {
                        return "#";
                    }
                }), RegexOptions.Multiline);

                var matchStrs = AppService.Instance.GetStatByMatchStr();
                var found = matchStrs.ContainsKey(withPlaceholders) ? matchStrs[withPlaceholders] : null;

                if (found != null) {
                    var values = matches.Select(str => double.Parse(str) * (found.Matcher.Negate ? -1 : 1))
                        .ToList();

                    return new ItemModifier() {
                        Stat = found.Stat,
                        StatMatchers = found.Matchers,
                        String = found.Matcher.String,
                        Ref = found.Matcher.Ref,
                        Negate = found.Matcher.Negate,
                        Option = found.Matcher.Option,
                        Condition = found.Matcher.Condition,
                        Values = values.Count > 0 ? values : new List<double>(),
                        Type = ItemModifierType.None
                    };
                }
            }

            return null;
        }

        private List<string> SectionToStatString(List<string> section) {
            List<string> stats = new List<string>();

            for (int i = 0; i < section.Count; ++i) {
                if (i + 1 < section.Count) {
                    var stat1 = section[i];
                    var stat2 = section[i + 1];

                    if (stat1.StartsWith("Added Small Passive Skills grant: ")) {
                        stat1 = stat1.Substring("Added Small Passive Skills grant: ".Length);
                    }

                    if (stat2.StartsWith("Added Small Passive Skills grant: ")) {
                        stat2 = stat2.Substring("Added Small Passive Skills grant: ".Length);
                    }

                    stats.Add($"{stat1}\n{stat2}");
                    stats.Add(section[i]);
                } else {
                    stats.Add(section[i]);
                }
            }

            return stats;

            //var idx = 0;
            //var multi = (idx + 1) < section.Count;

            //while (idx < section.Count) {
            //    string str;

            //    if (multi) {
            //        var lines = new List<string>() {
            //            section[idx],
            //            section[idx+1]
            //        };

            //        if (lines.All(l => l.StartsWith("Added Small Passive Skills grant: "))) {
            //            lines[1] = lines[1].Substring("Added Small Passive Skills grant: ".Length);
            //        }

            //        str = string.Join("\n", lines);
            //    } else {
            //        str = section[idx];
            //    }

            //    bool isParsed = string.IsNullOrEmpty(str);
            //    yield return str;

            //    if (isParsed) {
            //        idx += multi ? 2 : 1;
            //        multi = (idx + 1) < section.Count;
            //    } else {
            //        if (multi) {
            //            multi = false;
            //        } else {
            //            ++idx;
            //            multi = (idx + 1) < section.Count;
            //        }
            //    }

            //}
        }
        #endregion

        #region Public methods
        public void ParseClipboardLine(string line) {
            var evt = ParseLine(line, false);

            if (evt != null) {
                if (evt.EvenType == Enums.ChatEventEnum.Offer) {
                    var offer = (Offer)evt;

                    if (offer.IsOutgoing) {
                        OnNewChatEventParsed(offer);
                    }
                }
            }
        }

        public void ParseClientLine(string aline) {
            if (LastOffersLines.Contains(aline)) {
                return;
            }

            var evt = ParseLine(aline);

            if (evt != null) {
                OnNewChatEventParsed(evt);
            }
        }

        public Item ParseItem(string data) {
            string[] lines = Regex.Split(data, @"\r?\n");
            lines = lines.Take(lines.Length - 1).ToArray();

            List<List<string>> sections = new List<List<string>>();
            sections.Add(new List<string>());

            lines.Aggregate(sections[0], (section, line) => {
                if (line != SEPARATOR) {
                    section.Add(line);
                    return section;
                } else {
                    var newSection = new List<string>();
                    sections.Add(newSection);
                    return newSection;
                }
            });

            sections = sections.FindAll(s => s.Count > 0);

            if (sections[0][1] == CANNOT_USER_ITEM_WORD) {
                sections[1].Insert(0, sections[0][0]);
                sections.RemoveAt(0);
            }

            var item = ParseItemSection(sections[0]);

            if (item == null) {
                return null;
            }

            sections.RemoveAt(0);

            foreach (var parser in ItemParsers) {
                for (int i = 0; i < sections.Count; ++i) {
                    var result = parser(sections[i], item);

                    if (result == SECTION_PARSED) {
                        sections.RemoveAt(i);
                        --i;
                        break;
                    } else if (result == PARSER_SKIPPED) {
                        break;
                    }
                }
            }

            item.RawText = data;

            return item;
        }

        public void Start() {
            DoCleanBuffer();
            SetupItemParsers();
        }
        #endregion
    }
}
