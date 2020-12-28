using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Core.DTOs;

namespace Menagerie.Core.Services {
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

        private const string CONFIG_DB_FILE_PATH = @".\Menagerie.db";

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

        public ConfigDto GetConfig() {
            ConfigDto dto;

            using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                var collection = db.GetCollection<ConfigDto>("config");
                dto = collection.FindOne(e => true);
            }

            return dto;
        }

        public bool SetConfig(ConfigDto config) {
            bool result = false;

            using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                var collection = db.GetCollection<ConfigDto>("config");

                if (collection.Update(config)) {
                    result = true;
                }
            }

            return result;
        }
    }
}
