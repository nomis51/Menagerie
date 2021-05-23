using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Extensions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services
{
    public class PoeNinjaService : IService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PoeNinjaService));

        private static readonly object LockCurrencyCacheAccess = new();

        #region Constants

        private int _cacheExpirationTimeMinutes = 30;
        private readonly Uri _poeNinjaApiBaseUrl = new("https://poe.ninja");
        private const string PoeNinjaApiCurrency = "api/data/currencyoverview";

        #endregion

        #region Members

        private PoeNinjaCaches _oldCache;
        private PoeNinjaCaches _cache = new();
        private readonly HttpService _httpService;
        private bool _cacheUpdating;

        #endregion

        #region Props

        public bool CacheReady { get; private set; }

        #endregion

        #region Constructors

        public PoeNinjaService()
        {
            Log.Trace("Initializing PoeNinjaService");
            _httpService = new HttpService(_poeNinjaApiBaseUrl);
        }

        #endregion

        #region Private methods

        private void AutoUpdateCache(bool skipFirstUpdate = false, bool setOldCache = false)
        {
            Log.Trace("Starting auto cache update");
            while (true)
            {
                if (!skipFirstUpdate)
                {
                    if (setOldCache)
                    {
                        Log.Trace("Backup old cache");
                        lock (LockCurrencyCacheAccess)
                        {
                            _oldCache = _cache.Copy();
                        }
                    }

                    Log.Trace("Updating cache");
                    _cacheUpdating = true;
                    Task.Run(UpdateCurrencyCache).Wait();
                    _cache.UpdateTime = DateTime.Now;
                    _cacheUpdating = false;

                    SaveCache();
                }
                else
                {
                    Log.Trace("Skipping first cache update");
                    _cache = _oldCache.Copy();
                }

                CacheReady = true;
                skipFirstUpdate = false;
                setOldCache = true;

                Thread.Sleep(_cacheExpirationTimeMinutes * 60 * 1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void UpdateCurrencyCache()
        {
            Log.Trace("Updating currency cache");
            lock (LockCurrencyCacheAccess)
            {
                try
                {
                    var response = _httpService.Client
                        .GetAsync(
                            $"/{PoeNinjaApiCurrency}?league={AppService.Instance.GetConfig().CurrentLeague}&type=Currency&language=en")
                        .Result;
                    var result = HttpService.ReadResponse<PoeNinjaResult<PoeNinjaCurrency>>(response).Result;
                    var currencies = result.Lines.ToDictionary(line => line.CurrencyTypeName,
                        line => new List<PoeNinjaCurrency>() {line});

                    Log.Trace($"Poe Ninja returned {currencies.Count} currencies");

                    _cache.Currency = new PoeNinjaCache<PoeNinjaCurrency>()
                    {
                        Language = result.Language,
                        Map = currencies
                    };
                }
                catch (Exception e)
                {
                    Log.Error("Error while updating currency cache", e);
                }
            }
        }

        private static double GetCurrencyChaosValue(PoeNinjaCache<PoeNinjaCurrency> cache, string currencyName)
        {
            Log.Trace($"Getting currency chaos value for {currencyName}");
            if (cache == null)
            {
                return 0.0d;
            }

            return cache.Map.ContainsKey(currencyName) ? cache.Map[currencyName][0].Receive.Value : 0.0d;
        }

        private void SaveCache()
        {
            Log.Trace("Saving cache");
            AppService.Instance.SavePoeNinjaCaches(_cache);
        }

        private void LoadCache()
        {
            Log.Trace("Loading cache");
            _oldCache = AppService.Instance.GetPoeNinjaCaches();

            if (_oldCache != null)
            {
                Log.Trace("Existing cache found");
            }
        }

        #endregion

        #region Public methods

        public double GetCurrencyChaosValue(string currencyName)
        {
            if (_cacheUpdating)
            {
                if (_oldCache == null)
                {
                    return 0.0d;
                }

                Log.Trace($"Getting chaos value of {currencyName} from old cache");

                return GetCurrencyChaosValue(_oldCache.Currency, currencyName);
            }
            else
            {
                lock (LockCurrencyCacheAccess)
                {
                    Log.Trace($"Getting chaos value of {currencyName} from current cache");
                    return GetCurrencyChaosValue(_cache.Currency, currencyName);
                }
            }
        }

        public void Start()
        {
            Log.Trace("Starting PoeNinjaService");

            _cacheExpirationTimeMinutes = AppService.Instance.GetConfig().PoeNinjaUpdateRate;

            LoadCache();
            Task.Run(() => AutoUpdateCache(_oldCache != null &&
                                           (DateTime.Now - _oldCache.UpdateTime).TotalMinutes <
                                           _cacheExpirationTimeMinutes));
        }

        #endregion
    }
}