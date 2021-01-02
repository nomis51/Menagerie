using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Extensions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class PoeNinjaService : IService {
        private readonly static ILog log = LogManager.GetLogger(typeof(PoeNinjaService));

        //private static object _lockUpdatePoeNinjaItemsLoop = new object();
        private static object _lockCurrencyCacheAccess = new object();
        //private static object _lockItemsCacheAccess = new object();

        #region Constants
        private int CACHE_EXPIRATION_TIME_MINS = 30;
        private readonly Uri POE_NINJA_API_BASE_URL = new Uri("https://poe.ninja");
        private const string POE_NINJA_API_CURRENCY = "api/data/currencyoverview";
        //private const string POE_NINJA_API_ITEM = "api/data/itemoverview";

        //private List<string> ItemTypes = new List<string>() {
        //    "Oil",
        //    "Incubator",
        //    "Scarab",
        //    "Fossil",
        //    "Resonator",
        //    "Essence",
        //    "DivinationCard",
        //    "Prophecy",
        //    "SkillGem",
        //    "UniqueMap",
        //    "Map",
        //    "UniqueJewel",
        //    "UniqueFlask",
        //    "UniqueWeapon",
        //    "UniqueArmour",
        //    "UniqueAccessory",
        //    "Beast"
        //};
        #endregion

        #region Members
        private PoeNinjaCaches OldCache = null;
        private PoeNinjaCaches Cache = new PoeNinjaCaches();
        private HttpService _httpService;
        private bool CacheUpdating = false;
        #endregion

        #region Props
        public bool CacheReady { get; private set; } = false;
        #endregion

        #region Constructors
        public PoeNinjaService() {
            log.Trace("Initializing PoeNinjaService");
            _httpService = new HttpService(POE_NINJA_API_BASE_URL);
        }
        #endregion

        #region Private methods
        private void AutoUpdateCache(bool skipFirstUpdate = false, bool setOldCache = false) {
            log.Trace("Starting auto cache update");
            while (true) {
                if (!skipFirstUpdate) {
                    if (setOldCache) {
                        log.Trace("Backup old cache");
                        OldCache = Cache.Copy();
                    }

                    log.Trace("Updating cache");
                    CacheUpdating = true;
                    Task.Run(() => UpdateCurrencyCache()).Wait();
                    // Task.Run(() => UpdateItemsCache()).Wait();
                    Cache.UpdateTime = DateTime.Now;
                    CacheUpdating = false;

                    SaveCache();
                } else {
                    log.Trace("Skipping first cache update");
                    Cache = OldCache.Copy();
                }

                CacheReady = true;
                skipFirstUpdate = false;
                setOldCache = true;

                Thread.Sleep(CACHE_EXPIRATION_TIME_MINS * 60 * 1000);
            }
        }

        private void UpdateCurrencyCache() {
            log.Trace("Updating currency cache");
            lock (_lockCurrencyCacheAccess) {
                var response = _httpService.Client.GetAsync($"/{POE_NINJA_API_CURRENCY}?league={AppService.Instance.GetConfig().CurrentLeague}&type=Currency&language=en").Result;
                PoeNinjaResult<PoeNinjaCurrency> result = _httpService.ReadResponse<PoeNinjaResult<PoeNinjaCurrency>>(response).Result;
                Dictionary<string, List<PoeNinjaCurrency>> currencies = new Dictionary<string, List<PoeNinjaCurrency>>();

                foreach (var line in result.Lines) {
                    currencies.Add(line.CurrencyTypeName, new List<PoeNinjaCurrency>() { line });
                }

                log.Trace($"Poe Ninja returned {currencies.Count} currencies");

                Cache.Currency = new PoeNinjaCache<PoeNinjaCurrency>() {
                    Language = result.Language,
                    Map = currencies
                };
            }
        }

        //private void UpdateItemsCache() {
        //    lock (_lockItemsCacheAccess) {
        //        Dictionary<string, PoeNinjaResult<PoeNinjaItem>> results = new Dictionary<string, PoeNinjaResult<PoeNinjaItem>>();

        //        Cache.Items = new Dictionary<string, PoeNinjaCache<PoeNinjaItem>>();

        //        Parallel.For(0, ItemTypes.Count, (i) => {
        //            var response = _httpService.Client.GetAsync($"/{POE_NINJA_API_ITEM}?league={AppService.Instance.GetConfig().CurrentLeague}&type={ItemTypes[i]}&language=en").Result;
        //            PoeNinjaResult<PoeNinjaItem> result = _httpService.ReadResponse<PoeNinjaResult<PoeNinjaItem>>(response).Result;

        //            lock (_lockUpdatePoeNinjaItemsLoop) {
        //                results.Add(ItemTypes[i], result);
        //            }
        //        });

        //        foreach (var bulk in results) {
        //            foreach (var line in bulk.Value.Lines) {
        //                if (!Cache.Items.ContainsKey(bulk.Key)) {
        //                    Cache.Items.Add(bulk.Key, new PoeNinjaCache<PoeNinjaItem>());
        //                }

        //                if (Cache.Items[bulk.Key].Map == null) {
        //                    Cache.Items[bulk.Key].Map = new Dictionary<string, List<PoeNinjaItem>>();
        //                }

        //                if (!Cache.Items[bulk.Key].Map.ContainsKey(line.Name)) {
        //                    Cache.Items[bulk.Key].Map.Add(line.Name, new List<PoeNinjaItem>() { line });
        //                } else {
        //                    Cache.Items[bulk.Key].Map[line.Name].Add(line);
        //                }
        //            }
        //        }
        //    }
        //}

        //public double GetItemChaosValue(PoeNinjaCache<PoeNinjaItem> cache, string itemName) {
        //    if (cache == null) {
        //        return 0.0d;
        //    }


        //    if (cache.Map.ContainsKey(itemName)) {
        //        return cache.Map[itemName][0].ChaosValue;
        //    }

        //    return 0.0d;
        //}

        private double GetCurrencyChaosValue(PoeNinjaCache<PoeNinjaCurrency> cache, string currencyName) {
            log.Trace($"Getting currency chaos value for {currencyName}");
            if (cache == null) {
                return 0.0d;
            }

            if (cache.Map.ContainsKey(currencyName)) {
                return cache.Map[currencyName][0].Receive.Value;
            }

            return 0.0d;
        }

        private void SaveCache() {
            log.Trace("Saving cache");
            AppService.Instance.SavePoeNinjaCaches(Cache);
        }

        private void LoadCache() {
            log.Trace("Loading cache");
            OldCache = AppService.Instance.GetPoeNinjaCaches();

            if (OldCache != null) {
                log.Trace("Existing cache found");
            }
        }
        #endregion

        #region Public methods
        //public double GetItemChaosValue(string itemName, string itemType) {
        //    if (CacheUpdating) {
        //        if (OldCache == null) {
        //            return 0.0d;
        //        }

        //        if (OldCache.Items.ContainsKey(itemType)) {
        //            return GetItemChaosValue(OldCache.Items[itemType], itemName);
        //        }
        //    } else {
        //        lock (_lockItemsCacheAccess) {
        //            if (Cache.Items.ContainsKey(itemType)) {
        //                return GetItemChaosValue(Cache.Items[itemType], itemName);
        //            }
        //        }
        //    }

        //    return 0.0d;
        //}

        public double GetCurrencyChaosValue(string currencyName) {
            if (CacheUpdating) {
                if (OldCache == null) {
                    return 0.0d;
                }

                log.Trace($"Getting chaos value of {currencyName} from old cache");

                return GetCurrencyChaosValue(OldCache.Currency, currencyName);
            } else {
                lock (_lockCurrencyCacheAccess) {
                    log.Trace($"Getting chaos value of {currencyName} from current cache");
                    return GetCurrencyChaosValue(Cache.Currency, currencyName);
                }
            }
        }

        public void Start() {
            log.Trace("Starting PoeNinjaService");

            CACHE_EXPIRATION_TIME_MINS = AppService.Instance.GetConfig().PoeNinjaUpdateRate;

            LoadCache();
            Task.Run(() => AutoUpdateCache(OldCache != null && (DateTime.Now - OldCache.UpdateTime).TotalMinutes < CACHE_EXPIRATION_TIME_MINS));
        }
        #endregion
    }
}