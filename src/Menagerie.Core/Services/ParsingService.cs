﻿using Newtonsoft.Json;
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
using Menagerie.Core.Enums;
using log4net;
using Menagerie.Core.Extensions;

namespace Menagerie.Core {
    public class ParsingService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(ParsingService));
        private const string ITEM_NAME_START_WORD = "to buy your ";
        private const string ITEM_NAME_END_WORD = " listed for ";
        private const string ITEM_NAME_ALTERNATE_END_WORD = " for my ";
        private const string CURRENCY_END_WORD = " in ";
        private const string TRADE_ACCEPTED_MSG = "trade accepted";
        private const string TRADE_CANCELLED_MSG = "trade cancelled";
        private const string PLAYER_NOT_FOUND = "player not in this area";
        private const string PLAYER_JOINED_MSG = " has joined the area";
        private const string AREA_JOINED = "you have entered ";
        private const int MAX_OFFER_LINE_BUFFER = 20;
        private const int MAX_BUFFER_LIFE_MINS = 5;
        #endregion

        #region Members
        private List<string> LastOffersLines = new List<string>();
        private List<DateTime> LastOffersTimes = new List<DateTime>();
        private static int Id = -1;
        #endregion

        #region Constructors
        public ParsingService() {
            log.Trace("Initializing ParsingService");
        }
        #endregion

        #region Private methods
        private async void DoCleanBuffer() {
            log.Trace("Starting clean buffer");
            while (true) {
                await Task.Delay(MAX_BUFFER_LIFE_MINS * 1000 * 60);
                CleanBuffer();
            }
        }

        private void OnNewChatEventParsed(Models.ChatEvent evt) {
            log.Trace("New chat event");
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

                case ChatEventEnum.AreaJoined:
                    AppService.Instance.StashApiUpdated();
                    break;

                default:
                    AppService.Instance.NewChatEvent(evt.EvenType);
                    break;
            }
        }

        private ChatEvent ParseLine(string aline, bool isClientFileLine = true) {
            log.Trace($"Parsing line {aline} from {(isClientFileLine ? "Client file" : "Clipboard")}");
            ChatEvent evt = null;
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
                bool alternatePlayerEndIndex = false;
                int playerStartIndex = line.IndexOf("@From ");

                if (playerStartIndex == -1) {
                    playerStartIndex = line.IndexOf("@To ");

                    if (playerStartIndex == -1) {
                        playerStartIndex = line.IndexOf("@");
                        alternatePlayerEndIndex = true;

                        if (playerStartIndex == -1) {
                            continue;
                        }
                    }

                    offer.IsOutgoing = true;
                    playerStartIndex += 4;
                } else {
                    playerStartIndex += 6;
                }

                int playerEndIndex = -1;

                playerEndIndex = line.IndexOf(alternatePlayerEndIndex ? " Hi" : ": ", playerStartIndex);

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
                    leagueEndIndex = leagueStartIndex + offer.League.Length;
                } else {
                    offer.League = line.Substring(leagueStartIndex, leagueEndIndex - leagueStartIndex);
                }

                offer.League = Regex.Replace(offer.League, @"[0-9\.]", "");

                var config = AppService.Instance.GetConfig();
                if (config.AutoWhisperOutOfLeague && offer.League != config.CurrentLeague) {
                    AppService.Instance.SendChatMessage($"@{offer.PlayerName} {AppService.Instance.ReplaceVars(config.OutOfLeagueWhisper, offer)}");
                    return null;
                }

                // Item location
                int itemTabStartIndex = line.IndexOf("(stash tab \"", leagueEndIndex);
                int topEndIndex = -1;

                if (itemTabStartIndex != -1) {
                    itemTabStartIndex += "(stash tab \"".Length;
                    int itemTabEndIndex = line.IndexOf("\"; position: left ", itemTabStartIndex);

                    if (itemTabEndIndex != -1) {
                        offer.StashTab = line.Substring(itemTabStartIndex, itemTabEndIndex - itemTabStartIndex);

                        int leftStartIndex = itemTabEndIndex + "\"; position: left ".Length;
                        int leftEndIndex = line.IndexOf(", top ", leftStartIndex);

                        if (leftEndIndex != -1) {
                            int x = int.Parse(line.Substring(leftStartIndex, leftEndIndex - leftStartIndex));

                            int topStartIndex = leftEndIndex + ", top ".Length;
                            topEndIndex = line.IndexOf(")", topStartIndex);

                            if (topEndIndex != -1) {
                                int y = int.Parse(line.Substring(topStartIndex, topEndIndex - topStartIndex));
                                ++topEndIndex;
                                offer.Position = new System.Drawing.Point(x, y);
                            }
                        }
                    }
                }

                // Aditionnal notes
                if ((topEndIndex != -1 && topEndIndex + 1 < line.Length) || (leagueEndIndex < line.Length)) {
                    offer.Notes = line.Substring(topEndIndex == -1 ? leagueEndIndex : topEndIndex);
                }

                offer.Id = NextId();

                evt = offer;
                processed = true;
            }

            if (!processed) {
                log.Trace("Line wasn't an offer, trying to parse has other events");
                var line = aline.ToLower();

                if (line.IndexOf(TRADE_ACCEPTED_MSG) != -1) {
                    evt = new ChatEvent() { EvenType = Enums.ChatEventEnum.TradeAccepted };
                } else if (line.IndexOf(TRADE_CANCELLED_MSG) != -1) {
                    evt = new ChatEvent() { EvenType = Enums.ChatEventEnum.TradeCancelled };
                } else if (line.IndexOf(PLAYER_JOINED_MSG) != -1) {
                    var startIndex = aline.IndexOf("] : ");
                    var endIndex = aline.IndexOf(PLAYER_JOINED_MSG);

                    if (startIndex != -1 && endIndex != -1) {
                        startIndex += "] : ".Length;
                        evt = new JoinEvent(aline.Substring(startIndex, endIndex - startIndex));
                    }
                } else if (line.ToLower().IndexOf(AREA_JOINED) != -1 && line.ToLower().IndexOf("hideout") == -1) {
                    evt = new ChatEvent() { EvenType = ChatEventEnum.AreaJoined };
                 }
            }

            return evt;
        }

        private int NextId() {
            if(Id == -1) {
                Id = AppService.Instance.GetLastOfferId();
            }

            return ++Id; 
        }

        private void CleanBuffer() {
            log.Trace("Cleaning buffer");
            for (int i = 0; i < LastOffersTimes.Count; ++i) {
                if ((DateTime.Now - LastOffersTimes.ElementAt(i)).TotalMinutes >= MAX_BUFFER_LIFE_MINS) {
                    LastOffersLines.RemoveAt(i);
                    LastOffersTimes.RemoveAt(i);
                    --i;
                }
            }
        }

        private void ToBuffer(string line) {
            log.Trace($"Adding {line} to buffer");
            try {
                LastOffersLines.Add(line);
                LastOffersTimes.Add(DateTime.Now);

                if (LastOffersLines.Count > MAX_OFFER_LINE_BUFFER) {
                    log.Trace("Max buffer size reached. Cleaing old entries");
                    LastOffersLines.RemoveAt(0);
                    LastOffersTimes.RemoveAt(0);
                }
            } catch (Exception e) {
                log.Error("Error while adding line to buffer", e);
            }
        }
        #endregion

        #region Public methods
        public void ParseTradeChatLine(string line, List<string> words) {
            TradeChatLine tradeChatLine = new TradeChatLine();

            // Time
            int timeIndex = line.IndexOf(" ");

            if (timeIndex == -1) {
                return;
            }

            timeIndex = line.IndexOf(" ", timeIndex + 1);

            if (timeIndex == -1) {
                return;
            }

            var strTime = line.Substring(0, timeIndex);

            if (string.IsNullOrEmpty(strTime) || string.IsNullOrWhiteSpace(strTime)) {
                return;
            }

            DateTime date;

            if (!DateTime.TryParse(strTime.Replace("/", "-"), out date)) {
                return;
            }

            tradeChatLine.Time = date;

            // Player name
            int playerNameStartIndex = line.IndexOf("$");

            if (playerNameStartIndex == -1) {
                return;
            }

            int playerNameEndIndex = line.IndexOf(":", playerNameStartIndex + 1);

            if (playerNameEndIndex == -1) {
                return;
            }

            tradeChatLine.PlayerName = line.Substring(playerNameStartIndex + 1, playerNameEndIndex - playerNameStartIndex - 1);

            // Highlighted whisper
            string whisper = line.Substring(line.IndexOf(": ") + 2);

            List<TradeChatWords> tradeWords = new List<TradeChatWords>();

            foreach (var word in words) {
                int index = 0;

                while ((index = whisper.IndexOf(word)) != -1) {
                    if (index != -1) {
                        int endIndex = index + word.Length;

                        if (endIndex <= whisper.Length) {
                            tradeWords.Add(new TradeChatWords() {
                                Highlighted = true,
                                Words = whisper.Substring(index, endIndex - index),
                                Index = index
                            });

                            whisper = $"{whisper.Substring(0, index)}~{whisper.Substring(endIndex)}";
                        }
                    }
                }
            }

            int currentIndex = 0;
            while (currentIndex < whisper.Length) {
                int nextIndex = whisper.IndexOf('~', currentIndex);

                if (nextIndex == -1) {
                    tradeWords.Add(new TradeChatWords() {
                        Words = whisper.Substring(currentIndex),
                        Index = currentIndex
                    });
                    break;
                }

                tradeWords.Add(new TradeChatWords() {
                    Words = whisper.Substring(currentIndex, nextIndex - currentIndex),
                    Index = currentIndex
                });

                currentIndex = nextIndex + 1;
            }

            tradeChatLine.Words = tradeWords.OrderBy(w => w.Index).ToList();

            AppService.Instance.NewTradeChatLine(tradeChatLine);
        }

        public void ParseClipboardLine(string line) {
            log.Trace($"Parsing clipboard line {line}");

            if (AppService.Instance.GetConfig().AutoWhisper) {
                var evt = ParseLine(line, false);

                if (evt != null) {
                    if (evt.EvenType == ChatEventEnum.Offer) {
                        var offer = (Offer)evt;

                        if (offer.IsOutgoing) {
                            AppService.Instance.SendChatMessage(line.Substring(line.IndexOf("@")), 500);
                        }
                    }
                }
            }
        }

        public void ParseClientLine(string aline) {
            log.Trace($"Parsing client file line {aline}");
            try {
                int index = aline.IndexOf("@From ");
                bool isTo = false;

                if (index == -1) {
                    index = aline.IndexOf("@To ");

                    if (index == -1) {
                        index = aline.IndexOf("@");

                        if (index == -1) {
                            index = aline.IndexOf("]") + 2;
                        } else {
                            ++index;
                        }
                    } else {
                        isTo = true;
                        index += 4;
                    }
                } else {
                    index += 6;
                }

                if (!isTo && LastOffersLines.Contains(aline.Replace(": Hi", " Hi").Substring(index))) {
                    return;
                }

                var evt = ParseLine(aline);

                if (evt != null) {
                    if (!isTo && evt.EvenType == ChatEventEnum.Offer) {
                        ToBuffer(aline.Replace(": Hi", " Hi").Substring(index));
                    }

                    OnNewChatEventParsed(evt);
                }
            } catch (Exception e) {
                log.Error("Error while parsing client file", e);
            }
        }

        public void Start() {
            log.Trace("Starting ParsingService");
            DoCleanBuffer();
        }
        #endregion
    }
}
