using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class ClientFileService : IService {
        #region Members
        private long EndOfFile = 0;
        #endregion

        #region Constructors
        public ClientFileService() {
        }
        #endregion

        #region Handlers

        #endregion

        #region Private methods
        public void StartWatching() {
            SetEndOfFile();
            Watch();
        }

        private async void Watch() {
            while (true) {
                List<string> newLines = new List<string>();

                do {
                    try {
                        await Task.Delay(500);

                        newLines = this.ReadNewLines();
                    } catch { }
                }
                while (newLines.Count() == 0);

                foreach (var line in newLines) {
                    AppService.Instance.NewClientFileLine(line);
                }
            }
        }

        private void SetEndOfFile() {
            var file = File.Open(AppService.Instance.GetClientFilePath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

            var file = File.Open(AppService.Instance.GetClientFilePath(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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
        #endregion

        #region Public methods
        public void Test() {
            foreach (var line in new List<string> {
                  "2020/12/26 14:34:49 22374687 b60 [INFO Client 10844] @From KarmicVD: Hi, I'd like to buy your 163 chaos for my 10 exalted in Harvest.",
                "2020/08/16 18:33:53 22318140 b60 [INFO Client 10844] @From SpecialSoldierTT: Hi, I would like to buy your Fertile Catalyst listed for 899 chaos in Harvest",
                "2020/08/16 18:30:07 22092156 b60 [INFO Client 10844] @From <ZanaDP> ByHisMuscularGoldenArse: Hi, I would like to buy your Primal Scrabbler Grain listed for 19.5 chaos in Harvest",
                "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9.5 chaos in Harvest",
                 "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9 chaos in Harvest",
                  "2020/08/16 18:30:31 22116890 b60 [INFO Client 10844] @From Havhdfun: Hi, I would like to buy your Primal Scrabbler Grain listed for 9345 chaos in Harvest",
            }) {
                AppService.Instance.NewClientFileLine(line);
            }
        }

        public void Start() {
            
        }
        #endregion


    }
}
