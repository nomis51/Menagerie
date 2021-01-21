using System;
using System.Collections.Generic;
using System.Linq;
using Menagerie.Core.Models;
using Menagerie.Core.Abstractions;
using LiteDB;
using System.Linq.Expressions;
using log4net;
using Menagerie.Core.Extensions;

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
        private readonly LiteDatabase _db;
        #endregion

        #region Constructors
        public AppDataService() {
            log.Trace("Initializing AppDataService");
            _db = new LiteDatabase(DB_FILE_PATH);
        }
        #endregion

        private void EnsureDefaultData() {
            if (GetDocument<Config>(COLLECTION_CONFIG) == null) {
                log.Trace("Creating initial db data");
                InsertDocument<Config>(COLLECTION_CONFIG, new Config() {
                    PlayerName = "",
                    CurrentLeague = "Standard",
                    OnlyShowOffersOfCurrentLeague = true,
                    FilterSoldOffers = true,
                    BusyWhisper = "I'm busy right now, I'll whisper you for the \"{item}\" when I'm ready",
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
                    ChaosRecipeEnabled = false
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
            EnsureDefaultData();
        }
    }
}
