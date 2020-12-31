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
                    OnlyShowOffersOfCurrentLeague = false
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
