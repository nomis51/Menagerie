using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toucan.Core.Models;

namespace Toucan.Core {
    public class Parser {
        public delegate void NewOfferHandler(Offer offer);
        public event NewOfferHandler OnNewOffer;

        public delegate void NewChatEvent(ChatEvent type);
        public event NewChatEvent OnNewChatEvent;

        public delegate void NewPlayerJoined(string playerName);
        public event NewPlayerJoined OnNewPlayerJoined;

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

        private List<string> LastOffersLines = new List<string>();
        private List<DateTime> LastOffersTimes = new List<DateTime>();
        private static int Id = 0;

        public Parser() {
            DoCleanBuffer();
        }

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
                case ChatEvent.Offer:
                    OnNewOffer((Offer)evt);
                    break;

                case ChatEvent.PlayerJoined:
                    OnNewPlayerJoined(((JoinEvent)evt).PlayerName);
                    break;

                default:
                    OnNewChatEvent(evt.EvenType);
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
                offer.CurrencyImageLink = CurrencyHandler.GetCurrencyImageLink(offer.Currency);

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
                if (aline.IndexOf(TRADE_ACCEPTED_MSG) != -1) {
                    evt = new Models.ChatEvent() { EvenType = ChatEvent.TradeAccepted };
                } else if (aline.IndexOf(TRADE_CANCELLED_MSG) != -1) {
                    evt = new Models.ChatEvent() { EvenType = ChatEvent.TradeCancelled };
                } else if (aline.IndexOf(PLAYER_JOINED_MSG) != -1) {
                    var startIndex = aline.IndexOf("] ");
                    var endIndex = aline.IndexOf(PLAYER_JOINED_MSG);
                    if (startIndex != -1 && endIndex != -1) {
                        evt = new JoinEvent(aline.Substring(startIndex + 2, endIndex - startIndex - 2));
                    }
                }
            }

            return evt;
        }


        public void ParseClipboardLine(string line) {
            var evt = ParseLine(line, false);

            if (evt != null) {
                if (evt.EvenType == ChatEvent.Offer) {
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
    }
}
