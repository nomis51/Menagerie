using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Core.DTOs;
using Menagerie.Core.Abstractions;

namespace Menagerie.Core.Services {
    public class ConfigService : IService {
        #region Members
        private static object LockRead = new object();
        private static object LockWrite = new object();
        #endregion

        #region Constants
        private const string CONFIG_DB_FILE_PATH = @".\Menagerie.db";
        #endregion

        #region Constructors
        public ConfigService() {

        }
        #endregion

        #region Private methods
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
        #endregion

        #region Public methods
        public ConfigDto GetConfig() {
            ConfigDto dto;

            lock (LockRead) {
                using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                    var collection = db.GetCollection<ConfigDto>("config");
                    dto = collection.FindOne(e => true);
                }
            }

            return dto;
        }

        public bool SetConfig(ConfigDto config) {
            bool result = false;

            lock (LockWrite) {
                using (var db = new LiteDatabase(CONFIG_DB_FILE_PATH)) {
                    var collection = db.GetCollection<ConfigDto>("config");

                    if (collection.Update(config)) {
                        result = true;
                    }
                }
            }

            return result;
        }

        public void Start() {
            EnsureConfigCreated();
        }
        #endregion
    }
}
