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
using Newtonsoft.Json;

namespace Menagerie.Core.Services
{
    public class AppDataService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(AppDataService));
        private const string DataDir = "data/";
        public const string COLLECTION_CONFIG = DataDir + "config.json";
        public const string COLLECTION_TRADES = DataDir + "trades.json";
        public const string COLLECTION_POE_NINJA_CACHES = DataDir + "poeNinjaCaches.json";
        public const string COLLECTION_IMAGES = DataDir + "images.json";

        #endregion

        #region Members

        #endregion

        #region Constructors

        public AppDataService()
        {
            Log.Trace("Initializing AppDataService");

            if (!Directory.Exists(DataDir))
            {
                Directory.CreateDirectory(DataDir);
            }

            if (!File.Exists(COLLECTION_CONFIG))
            {
                File.WriteAllText(COLLECTION_CONFIG, "[]");
            }

            if (!File.Exists(COLLECTION_TRADES))
            {
                File.WriteAllText(COLLECTION_TRADES, "[]");
            }

            if (!File.Exists(COLLECTION_POE_NINJA_CACHES))
            {
                File.WriteAllText(COLLECTION_POE_NINJA_CACHES, "[]");
            }

            if (!File.Exists(COLLECTION_IMAGES))
            {
                File.WriteAllText(COLLECTION_IMAGES, "[]");
            }
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
            InsertDocument(COLLECTION_CONFIG, new Config()
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

        public List<T> GetDocuments<T>(string collectionName, Predicate<T> predicate = null) where T : IDocument
        {
            Log.Trace($"Reading db documents for {typeof(T)}");
            var elements = JsonConvert.DeserializeObject<List<T>>(File.ReadAllText(collectionName));

            return elements == null ? new List<T>() : predicate == null ? elements : elements.FindAll(predicate);
        }

        public T GetDocument<T>(string collectionName, Predicate<T> predicate = null) where T : IDocument
        {
            var docs = GetDocuments<T>(collectionName, predicate);
            return docs.FirstOrDefault();
        }

        public ObjectId InsertDocument<T>(string collectionName, T doc) where T : IDocument
        {
            Log.Trace($"Inserting db document for {typeof(T)}");

            var elements = GetDocuments<T>(collectionName);

            elements.Add(doc);

            File.WriteAllText(collectionName, JsonConvert.SerializeObject(elements));

            return doc.Id;
        }

        public bool UpdateDocument<T>(string collectionName, T doc) where T : IDocument
        {
            Log.Trace($"Updating db document for {typeof(T)}");
            var elements = GetDocuments<T>(collectionName);

            var index = elements.FindIndex(d => d.Id == doc.Id);

            if (index == -1) return false;

            elements[index] = doc;

            File.WriteAllText(collectionName, JsonConvert.SerializeObject(elements));

            return true;
        }

        public void DeleteAllDocument(string collectionName)
        {
            Log.Trace($"Deleting db documents for {collectionName}");
            File.WriteAllText(collectionName, "[]");
        }

        public void Start()
        {
            Log.Trace("Starting AppDataService");
            EnsureDefaultData();
        }
    }
}