using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Toucan.Core.Models;

namespace Toucan.Core.Services {
    public class AppDataService {
        #region Singleton
        private static AppDataService _instance;
        public static AppDataService Instance {
            get {
                if (_instance == null) {
                    _instance = new AppDataService();
                }

                return _instance;
            }
        }
        #endregion

        #region Constants
        private const string DATA_FOLDER = @".\data\";
        private const string STATS = "stats.json";
        private const string BASE_TYPES = "base-types.json";
        #endregion

        private AppDataService() { }

        public Dictionary<string, MatchStr> GetStatByMatchStr() {
            var data = File.ReadAllText(Path.Combine(DATA_FOLDER, STATS));
            return JsonConvert.DeserializeObject<Dictionary<string, MatchStr>>(data);
        }

        public Dictionary<string, BaseType> GetBaseTypes() {
            var data = File.ReadAllText(Path.Combine(DATA_FOLDER, BASE_TYPES));
            return JsonConvert.DeserializeObject<Dictionary<string, BaseType>>(data);
        }
    }

    public class MatchStr {
        public StatMatcher Matcher { get; set; }
        public Stat Stat { get; set; }
        public List<StatMatcher> Matchers { get; set; } = new List<StatMatcher>();
    }
}
