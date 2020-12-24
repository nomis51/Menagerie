using Toucan.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toucan.Core {
    public class ClientFileParser {
        #region Events
        public delegate void NewOfferHandler(Offer offer);
        public event NewOfferHandler OnNewOffer;

        public delegate void NewChatEvent(ChatEvent type);
        public event NewChatEvent OnNewChatEvent;

        public delegate void NewPlayerJoined(string playerName);
        public event NewPlayerJoined OnNewPlayerJoined;
        #endregion

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


        private int Id = 0;
        private string ClientFilePath;
        private int NbLines = 0;
        private List<string> LastOffersLines = new List<string>();
        private List<DateTime> LastOffersTimes = new List<DateTime>();

        public ClientFileParser(string clientFilePath) {
            this.ClientFilePath = clientFilePath;
            StartWatching();
        }

        private void StartWatching() {
            SetNbLines();
            Watch();
        }

        public void Test() {
            foreach (var line in new List<string> {
                  "2020/08/16 18:34:49 22374687 b60 [INFO Client 10844] @From KarmicVD: Hi, I'd like to buy your 163 chaos for my 10 exalted in Harvest.",
                "2020/08/16 18:33:53 22318140 b60 [INFO Client 10844] @From SpecialSoldierTT: Hi, I would like to buy your Fertile Catalyst listed for 899 chaos in Harvest",
                "2020/08/16 18:30:07 22092156 b60 [INFO Client 10844] @From <ZanaDP> ByHisMuscularGoldenArse: Hi, I would like to buy your Primal Scrabbler Grain listed for 19.5 chaos in Harvest",
                "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9.5 chaos in Harvest",
                 "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9 chaos in Harvest",
                  "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9345 chaos in Harvest",
            }) {
                var evt = ParseLine(line);
                OnNewOffer((Offer)evt);
            }
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

        private async void Watch() {
            while (true) {
                IEnumerable<string> newLines;

                do {
                    await Task.Delay(500);

                    CleanBuffer();

                    newLines = this.ReadNewLines();
                }
                while (newLines.Count() == 0);

                foreach (var line in newLines) {
                    this.OnNewLine(line);
                }
            }
        }

        private void OnNewLine(string line) {
            var evt = ParseLine(line);

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

        private int NextId() {
            return ++Id;
        }

        private Models.ChatEvent ParseLine(string aline) {
            if (LastOffersLines.Contains(aline)) {
                return null;
            }

            Models.ChatEvent evt = null;
            bool processed = false;

            foreach (var line in new List<string> { aline }) {
                Offer offer = new Offer();

                // Time
                int timeIndex = line.IndexOf(" ");
                timeIndex = line.IndexOf(" ", timeIndex + 1);
                offer.Time = DateTime.Parse(line.Substring(0, timeIndex).Replace("/", "-"));

                // Player name
                int playerStartIndex = line.IndexOf("@From ");

                if (playerStartIndex == -1) {
                    playerStartIndex = line.IndexOf("@");

                    if (playerStartIndex == -1) {
                        // Not a whisper
                        continue;
                    }

                    offer.IsOutgoing = true;
                    playerStartIndex += 1;
                } else {
                    playerStartIndex += 6;
                }

                int playerEndIndex = line.IndexOf(": ", playerStartIndex);

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

        private void SetNbLines() {
            var file = File.Open(ClientFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            NbLines = 0;

            StreamReader reader = new StreamReader(file);

            while (!reader.EndOfStream) {
                reader.ReadLine();
                ++NbLines;
            }

            reader.Close();
            file.Close();
        }

        private List<string> ReadNewLines() {
            List<string> lines = new List<string>();

            int currentLine = NbLines;

            SetNbLines();

            while (currentLine < NbLines) {
                string line = File.ReadLines(ClientFilePath).ElementAtOrDefault(currentLine);

                if (!string.IsNullOrEmpty(line)) {
                    lines.Add(line);
                }

                ++currentLine;
            }

            return lines;
        }
    }

    public enum ChatEvent {
        TradeAccepted,
        TradeCancelled,
        PlayerJoined,
        PlayerLeft,
        Offer
    }
}
