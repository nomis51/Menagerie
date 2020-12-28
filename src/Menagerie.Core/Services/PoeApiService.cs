using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Menagerie.Core.Services {
    public class PoeApiService {
        #region Singleton
        private static PoeApiService _instance;
        public static PoeApiService Instance {
            get {
                if (_instance == null) {
                    _instance = new PoeApiService();
                }

                return _instance;
            }
        }
        #endregion

        private const string POE_API_LEAGUES = "http://api.pathofexile.com/leagues?compact=1";

        private PoeApiService() { }

        public List<string> GetLeagues() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(POE_API_LEAGUES);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            string data = "";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
                data = reader.ReadToEnd();
            }

            return ParseLeagues(data);
        }

        private List<string> ParseLeagues(string data) {
            var json = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);
            return json.Select(l => l["id"])
                .ToList()
                .FindAll(n => n.IndexOf("SSF") == -1)
                .ToList();
        }
    }
}
