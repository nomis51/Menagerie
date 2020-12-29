using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class PoeNinjaService : Service {
        #region Constants
        private const int CACHE_EXPIRATION_TIME_MINS = 30;
        private readonly Uri POE_NINJA_API_BASE_URL = new Uri("https://poe.ninja");
        private const string POE_NINJA_API_CURRENCY = "api/data/currencyoverview";
        #endregion

        #region Members
        private PoeNinjaCache<PoeNinjaCurrency> CurrencyCache;
        private HttpService _httpService;
        #endregion

        #region Constructors
        public PoeNinjaService() {
            _httpService = new HttpService(POE_NINJA_API_BASE_URL);
        }
        #endregion

        #region Private methods
        private async Task UpdateCurrencyCache() {
            var response = _httpService.Client.GetAsync($"/{POE_NINJA_API_CURRENCY}?league={AppService.Instance.GetConfig().CurrentLeague}&type=Currency&language=en").Result;
            PoeNinjaResult<PoeNinjaCurrency> result = await _httpService.ReadResponse<PoeNinjaResult<PoeNinjaCurrency>>(response);
            Dictionary<string, PoeNinjaCurrency> currencies = new Dictionary<string, PoeNinjaCurrency>();

            foreach (var line in result.Lines) {
                currencies.Add(line.CurrencyTypeName, line);
            }

            CurrencyCache = new PoeNinjaCache<PoeNinjaCurrency>() {
                Language = result.Language,
                Map = currencies
            };
        }
        #endregion

        #region Public methods
        public double GetChaosValue(string currencyName) {
            if (CurrencyCache == null || (DateTime.Now - CurrencyCache.UpdateTime).TotalMinutes >= CACHE_EXPIRATION_TIME_MINS) {
                UpdateCurrencyCache().Wait();
            }

            if (CurrencyCache.Map.ContainsKey(currencyName)) {
                return CurrencyCache.Map[currencyName].Receive.Value;
            }

            return 0.0d;
        }
        #endregion
    }
}
