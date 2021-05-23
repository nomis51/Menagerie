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

namespace Menagerie.Core.Services
{
    public class AppDataService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(AppDataService));
        private const string DbFilePath = "Menagerie.db";
        public const string COLLECTION_CONFIG = "config";
        public const string COLLECTION_TRADES = "trades";
        public const string COLLECTION_POE_NINJA_CACHES = "poeNinjaCaches";
        public const string COLLECTION_IMAGES = "images";

        #endregion

        #region Members

        private readonly LiteDatabase _db;

        #endregion

        #region Constructors

        public AppDataService()
        {
            Log.Trace("Initializing AppDataService");

            if (!File.Exists(DbFilePath))
            {
                CopyOldConfig();
            }

            _db = new LiteDatabase(DbFilePath);
        }

        #endregion

        private static void CopyOldConfig()
        {
            Log.Trace("Looking for previous version db data");
            var foundConfig = false;
            var currentVersion = AppService.GetAppVersion();
            const string appFolderPath = "app-";

            var highestVersion = new AppVersion(0, 0, 0);
            var highestVersionPath = "";


            foreach (var dir in Directory.EnumerateDirectories(".."))
            {
                Log.Trace(dir);
                var appFolderPathIndex = dir.IndexOf(appFolderPath, StringComparison.Ordinal);

                if (appFolderPathIndex == -1)
                {
                    continue;
                }

                var versionStr = dir[(appFolderPathIndex + appFolderPath.Length)..];

                try
                {
                    if (versionStr != currentVersion.ToString())
                    {
                        var dbFile = $"{dir}\\Menagerie.db";

                        if (File.Exists(dbFile))
                        {
                            var versionSplits = versionStr.Split('.');

                            if (versionSplits.Length < 3)
                            {
                                continue;
                            }

                            var major = Convert.ToInt32(versionSplits[0]);
                            var minor = Convert.ToInt32(versionSplits[1]);
                            var build = Convert.ToInt32(versionSplits[2]);

                            if (major > highestVersion.Major ||
                                (major == highestVersion.Major && minor > highestVersion.Minor) ||
                                (major == highestVersion.Major && minor == highestVersion.Minor &&
                                 build > highestVersion.Build))
                            {
                                highestVersion.Major = major;
                                highestVersion.Minor = minor;
                                highestVersion.Build = build;
                                highestVersionPath = dbFile;
                                foundConfig = true;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            if (!foundConfig) return;
            try
            {
                File.Copy(highestVersionPath, "Menagerie.db");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void EnsureDefaultData()
        {
            if (GetDocument<Config>(COLLECTION_CONFIG) != null) return;
            Log.Trace("Creating initial db data");
            InsertDocument<Config>(COLLECTION_CONFIG, new Config()
            {
                PlayerName = "",
                CurrentLeague = "Standard",
                OnlyShowOffersOfCurrentLeague = true,
                FilterSoldOffers = true,
                BusyWhisper =
                    "I'm busy right now in {location}, I'll whisper you for the \"{item}\" when I'm ready",
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

        public List<T> GetDocuments<T>(string collectionName, Expression<Func<T, bool>> predicate = null)
        {
            Log.Trace($"Reading db documents for {typeof(T)}");
            return predicate == null
                ? _db.GetCollection<T>(collectionName)
                    .FindAll()
                    .ToList()
                : _db.GetCollection<T>(collectionName)
                    .Find(predicate)
                    .ToList();
        }

        public T GetDocument<T>(string collectionName, Expression<Func<T, bool>> predicate = null)
        {
            var docs = GetDocuments<T>(collectionName, predicate);
            return docs.FirstOrDefault();
        }

        public int InsertDocument<T>(string collectionName, T doc)
        {
            Log.Trace($"Inserting db document for {typeof(T)}");
            return _db.GetCollection<T>(collectionName)
                .Insert(doc);
        }

        public bool UpdateDocument<T>(string collectionName, T doc)
        {
            Log.Trace($"Updating db document for {typeof(T)}");
            return _db.GetCollection<T>(collectionName)
                .Update(doc);
        }

        public void DeleteAllDocument(string collectionName)
        {
            Log.Trace($"Deleting db documents for {collectionName}");
            _db.GetCollection(collectionName)
                .DeleteAll();
        }

        public void Start()
        {
            Log.Trace("Starting AppDataService");
            EnsureDefaultData();
        }
    }
}