﻿using Toucan.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toucan.Core {
    public class ClientFileParser {
        #region Events
        public delegate void NewLine(string line);
        public event NewLine OnNewLine;
        #endregion

        private string ClientFilePath;
        private long EndOfFile = 0;

        public ClientFileParser(string clientFilePath) {
            this.ClientFilePath = clientFilePath;
            StartWatching();
        }

        private void StartWatching() {
            SetEndOfFile();
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
                OnNewLine(line);
            }
        }

        private async void Watch() {
            while (true) {
                IEnumerable<string> newLines;

                do {
                    await Task.Delay(500);

                    newLines = this.ReadNewLines();
                }
                while (newLines.Count() == 0);

                foreach (var line in newLines) {
                    this.OnNewLine(line);
                }
            }
        }

        private void SetEndOfFile() {
            var file = File.Open(ClientFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            EndOfFile = file.Length - 1;
            file.Close();
        }

        private List<string> ReadNewLines() {
            List<string> lines = new List<string>();

            long currentPosition = EndOfFile;

            SetEndOfFile();

            if (currentPosition >= EndOfFile) {
                return lines;
            }

            var file = File.Open(ClientFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            file.Position = currentPosition;
            StreamReader reader = new StreamReader(file);

            while (!reader.EndOfStream) {
                string line = reader.ReadLine();

                if (!string.IsNullOrEmpty(line)) {
                    lines.Add(line);
                }
            }

            reader.Close();
            file.Close();

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
