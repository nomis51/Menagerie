using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Menagerie.Core.Models;
using Menagerie.Core.Abstractions;
using System.Threading.Tasks;
using LiteDB;
using System.Linq.Expressions;

namespace Menagerie.Core.Services {
    public class AppDataService : IService {
        #region Constants
        private const string DB_FILE_PATH = "Menagerie.db";

        public static readonly string COLLECTION_CONFIG = "config";
        #endregion

        #region Members
        private readonly LiteDatabase _db;
        #endregion

        #region Constructors
        public AppDataService() {
            _db = new LiteDatabase(DB_FILE_PATH);
        }
        #endregion

        private void EnsureDefaultData() {
            if (GetDocument<Config>(COLLECTION_CONFIG) == null) {
                InsertDocument<Config>(COLLECTION_CONFIG, new Config() {
                    PlayerName = "",
                    CurrentLeague = "Standard",
                    OnlyShowOffersOfCurrentLeague = false
                });
            }
        }

        public List<T> GetDocuments<T>(string collectionName, Expression<Func<T, bool>> predicate = null) {
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
            return _db.GetCollection<T>(collectionName)
                .Insert(doc);
        }

        public bool UpdateDocument<T>(string collectionName, T doc) {
            return _db.GetCollection<T>(collectionName)
                .Update(doc);
        }

        public void DeleteAllDocument(string collectionName) {
            _db.GetCollection(collectionName)
                .DeleteAll();
        }

        public void Start() {
            EnsureDefaultData();
        }
    }
}
