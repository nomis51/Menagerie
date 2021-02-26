using System;
using System.Collections.Generic;
using System.Linq;
using Menagerie.Core.Models;
using Menagerie.Core.Abstractions;
using LiteDB;
using System.Linq.Expressions;
using log4net;
using Menagerie.Core.Extensions;
using System.IO;

namespace Menagerie.Core.Services {
    public class AppDataService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(AppDataService));
        private const string DB_FILE_PATH = "Menagerie.db";
        public static readonly string COLLECTION_CONFIG = "config";
        public static readonly string COLLECTION_TRADES = "trades";
        public static readonly string COLLECTION_POE_NINJA_CACHES = "poeNinjaCaches";
        public static readonly string COLLECTION_IMAGES = "images";
        #endregion

        #region Members
        private LiteDatabase _db;
        #endregion

        #region Constructors
        public AppDataService() {
            log.Trace("Initializing AppDataService");

          
        }
        #endregion

        private void CopyOldConfig() {
            log.Trace("Looking for previous version db data");
            bool foundConfig = false;
            var currentVersion = AppService.Instance.GetAppVersion();
            string appFolderPath = "app-";

            AppVersion highestVersion = new AppVersion(0,0,0);
            string highestVersionPath = "";


            foreach (var dir in Directory.EnumerateDirectories("..")) {
                log.Trace(dir);
                int appFolderPathIndex = dir.IndexOf(appFolderPath);

                if (appFolderPathIndex == -1) {
                    continue;
                }

                string versionStr = dir.Substring(appFolderPathIndex + appFolderPath.Length);

                try {
                    if (versionStr != currentVersion.ToString()) {
                        string dbFile = $"{dir}\\Menagerie.db";

                        if (File.Exists(dbFile)) {
                            var versionSplits = versionStr.Split('.');

                            if (versionSplits.Length < 3) {
                                continue;
                            }

                            int major = Convert.ToInt32(versionSplits[0]);
                            int minor = Convert.ToInt32(versionSplits[1]);
                            int build = Convert.ToInt32(versionSplits[2]);

                            if (major > highestVersion.Major || (major == highestVersion.Major && minor > highestVersion.Minor) || (major == highestVersion.Major && minor == highestVersion.Minor && build > highestVersion.Build)) {
                                highestVersion.Major = major;
                                highestVersion.Minor = minor;
                                highestVersion.Build = build;
                                highestVersionPath = dbFile;
                                foundConfig = true;
                            }
                        }
                    }
                } catch (Exception e) {
                    log.Error(e);
                    continue;
                }
            }

            if (foundConfig) {
                try {
                    File.Copy(highestVersionPath, "Menagerie.db");
                } catch (Exception e) {
                    log.Error(e);
                    foundConfig = false;
                }

            }
        }

        private void EnsureDefaultData() {
            if (GetDocument<Config>(COLLECTION_CONFIG) == null) {
                log.Trace("Creating initial db data");
                InsertDocument<Config>(COLLECTION_CONFIG, new Config() {
                    PlayerName = "",
                    CurrentLeague = "Standard",
                    OnlyShowOffersOfCurrentLeague = true,
                    FilterSoldOffers = true,
                    BusyWhisper = "I'm busy right now in {location}, I'll whisper you for the \"{item}\" when I'm ready",
                    SoldWhisper = "I'm sorry, my \"{item}\" has already been sold",
                    StillInterestedWhisper = "Are you still interested in my \"{item}\" listed for {price}?",
                    ThanksWhisper = "Thank you and have fun!",
                    AutoThanks = true,
                    AutoWhisper = false,
                    AutoWhisperOutOfLeague = false,
                    OutOfLeagueWhisper = "Sorry, I'm busy in another league",
                    PoeNinjaUpdateRate = 30,
                    ChaosRecipeTabIndex = 3,
                    ChaosRecipeRefreshRate = 1,
                    ChaosRecipeMaxSets = 3,
                    ChaosRecipeEnabled = false,
                    ChaosRecipeOveralyDockMode = true
                });
            }
        }

        public List<T> GetDocuments<T>(string collectionName, Expression<Func<T, bool>> predicate = null) {
            log.Trace($"Reading db documents for {typeof(T)}");
            return predicate == null ?
                _db.GetCollection<T>(collectionName)
                .FindAll()
                .ToList() :
                _db.GetCollection<T>(collectionName)
                .Find(predicate)
                .ToList();
        }

        public T GetDocument<T>(string collectionName, Expression<Func<T, bool>> predicate = null) {
            var docs = GetDocuments<T>(collectionName, predicate);
            return docs.FirstOrDefault();
        }

        public int InsertDocument<T>(string collectionName, T doc) {
            log.Trace($"Inserting db document for {typeof(T)}");
            return _db.GetCollection<T>(collectionName)
                .Insert(doc);
        }

        public bool UpdateDocument<T>(string collectionName, T doc) {
            log.Trace($"Updating db document for {typeof(T)}");
            return _db.GetCollection<T>(collectionName)
                .Update(doc);
        }

        public void DeleteAllDocument(string collectionName) {
            log.Trace($"Deleting db documents for {collectionName}");
            _db.GetCollection(collectionName)
                .DeleteAll();
        }

        public void Start() {
            log.Trace("Starting AppDataService");
            if (!File.Exists(DB_FILE_PATH)) {
                CopyOldConfig();
            }

            _db = new LiteDatabase(DB_FILE_PATH);

            EnsureDefaultData();
        }
    }
}
