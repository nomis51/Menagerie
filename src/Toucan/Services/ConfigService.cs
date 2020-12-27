using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toucan.DTOs;
using Toucan.Models;

namespace Toucan.Services {
    public class ConfigService {
        #region Singleton
        private static ConfigService _instance;
        public static ConfigService Instance {
            get {
                if (_instance == null) {
                    _instance = new ConfigService();
                }

                return _instance;
            }
        }
        #endregion

        private const string CONFIG_DB_FILE_PATH = @".\toucan.db";

        private ConfigService() {
            EnsureConfigCreated();
        }

        private void EnsureConfigCreated() {
            using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                var collection = db.GetCollection<ConfigDto>("config");

                if (collection.FindOne(e => true) == null) {
                    collection.Insert(new ConfigDto() {
                        PlayerName = "",
                        CurrentLeague = "Standard",
                        OnlyShowOffersOfCurrentLeague = false
                    });
                }
            }
        }

        public Config GetConfig() {
            Config config;

            using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                var collection = db.GetCollection<ConfigDto>("config");
                var dto = collection.FindOne(e => true);
                config = new Config(dto);
            }

            return config;
        }

        public bool SetConfig(Config config) {
            bool result = false;

            using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                var collection = db.GetCollection<ConfigDto>("config");
                var dto = new ConfigDto(config);

                if (collection.Update(dto)) {
                    result = true;
                }
            }

            return result;
        }
    }
}
