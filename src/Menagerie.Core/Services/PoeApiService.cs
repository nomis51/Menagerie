using Menagerie.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class PoeApiService {
        #region Singleton
        private static PoeApiService _instance;
        public static PoeApiService Instance {
            get {
                if (_instance == null) {
                    _instance = new PoeApiService();
                }

                return _instance;
            }
        }
        #endregion

        private const int CACHE_EXPIRATION_TIME_MINS = 30;
        private const int NB_RESULT_PER_QUERY = 10;
        private readonly Uri ALT_POE_API_BASE_URL = new Uri("http://api.pathofexile.com");
        private readonly Uri POE_API_BASE_URL = new Uri("https://www.pathofexile.com");
        private readonly Uri POE_NINJA_API_BASE_URL = new Uri("https://poe.ninja");
        private const string POE_API_LEAGUES = "leagues?compact=1";
        private const string POE_API_TRADE = "api/trade/search";
        private const string POE_API_FETCH = "api/trade/fetch";
        private const string POE_NINJA_API_CURRENCY = "api/data/currencyoverview";

        private static readonly HttpClient Client = new HttpClient();
        private static readonly HttpClient AltClient = new HttpClient();
        private static readonly HttpClient PoeNinjaClient = new HttpClient();

        private PoeNinjaCache<PoeNinjaCurrency> CurrencyCache;

        private PoeApiService() {
            SetupClients();
        }

        private void SetupClients() {
            Client.BaseAddress = POE_API_BASE_URL;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");

            AltClient.BaseAddress = ALT_POE_API_BASE_URL;
            AltClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            AltClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            AltClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");

            PoeNinjaClient.BaseAddress = POE_NINJA_API_BASE_URL;
            PoeNinjaClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            PoeNinjaClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            PoeNinjaClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");
        }

        private async Task<T> ReadResponse<T>(HttpResponseMessage response) {
            if (response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();
                try {
                    return JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings() {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
                } catch (Exception e) {
                    var h = 0;
                }
            }

            return default(T);
        }

        public HttpContent SerializeBody<T>(T obj) {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public async Task<List<string>> GetLeagues() {
            var response = AltClient.GetAsync($"/{POE_API_LEAGUES}").Result;
            var result = await ReadResponse<List<Dictionary<string, string>>>(response);

            return ParseLeagues(result);
        }

        private List<string> ParseLeagues(List<Dictionary<string, string>> json) {
            return json.Select(l => l["id"])
                .ToList()
                .FindAll(n => n.IndexOf("SSF") == -1)
                .ToList();
        }

        private static IEnumerable<int> SteppedIterator(int startIndex, int endIndex, int stepSize) {
            for (int i = startIndex; i < endIndex; i = i + stepSize) {
                yield return i;
            }
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request) {
            var json = SerializeBody(request);

            var response = Client.PostAsync($"/{POE_API_TRADE}/{ConfigService.Instance.GetConfig().CurrentLeague}", json).Result;

            SearchResult result = await ReadResponse<SearchResult>(response);

            if (result == null || result.Error != null) {
                throw new Exception("Error while getting trade request results");
            }

            return result;
        }

        public async Task<PriceCheckResult> GetTradeResults(SearchResult search, Item item, int nbResults = 20) {
            List<FetchResult> results = new List<FetchResult>();
            object locker = new object();

            var loopResult = Parallel.ForEach(SteppedIterator(0, nbResults, NB_RESULT_PER_QUERY), async (i) => {
                var ids = search.Result.Skip(i)
                .Take(NB_RESULT_PER_QUERY)
                .ToList();
                var result = GetTradeResults(search.Id, ids).Result;

                lock (locker) {
                    results.Add(result);
                }
            });

            if (results.Count == 0) {
                return null;
            }

            if (results.Count > 1) {
                for (int i = 1; i < results.Count; ++i) {
                    results[0].Result.AddRange(results[i].Result);
                }
            }

            if (results[0].Result.Count > 0) {
                item.Icon = results[0].Result[0].Item.Icon;
            }

            return await CalculateChaosValue(new PriceCheckResult() {
                Item = null,
                Results = results[0].Result.Select(p => new PricingResult() {
                    Currency = p.Listing.Price.Currency,
                    Price = p.Listing.Price.Amount,
                    CurrencyImageLink = CurrencyHandler.GetCurrencyImageLink(p.Listing.Price.Currency),
                    PlayerName = p.Listing.Account.Name
                }).ToList()
            });
        }

        private async Task<PriceCheckResult> CalculateChaosValue(PriceCheckResult priceCheck) {
            if (CurrencyCache == null || (DateTime.Now - CurrencyCache.UpdateTime).TotalMinutes >= CACHE_EXPIRATION_TIME_MINS) {
                await UpdateCurrencyCache();
            }

            foreach (var result in priceCheck.Results) {
                if (CurrencyCache.Map.ContainsKey(result.Currency)) {
                    result.ChaosValue = CurrencyCache.Map[result.Currency].Receive.Value;
                }
            }

            return priceCheck;
        }

        private async Task UpdateCurrencyCache() {
            var response = PoeNinjaClient.GetAsync($"/{POE_NINJA_API_CURRENCY}?league={ConfigService.Instance.GetConfig().CurrentLeague}&type=Currency&language=en").Result;
            PoeNinjaResult<PoeNinjaCurrency> result = await ReadResponse<PoeNinjaResult<PoeNinjaCurrency>>(response);
            Dictionary<string, PoeNinjaCurrency> currencies = new Dictionary<string, PoeNinjaCurrency>();

            foreach (var line in result.Lines) {
                currencies.Add(line.CurrencyTypeName, line);
            }

            CurrencyCache = new PoeNinjaCache<PoeNinjaCurrency>() {
                Language = result.Language,
                Map = currencies
            };
        }

        private async Task<FetchResult> GetTradeResults(string queryId, List<string> resultIds) {
            var ids = string.Join(",", resultIds);
            var response = Client.GetAsync($"/{POE_API_FETCH}/{ids}?query={queryId}").Result;

            FetchResult result = await ReadResponse<FetchResult>(response);

            if (result == null) {
                throw new Exception("Error while getting trade results");
            }

            return result;
        }

        public TradeRequest CreateTradeRequest(Item item) {
            TradeRequest body = new TradeRequest() {
                Query = new TradeRequestQuery() {
                    Name = new TradeRequestType() {
                        Option = item.Name,
                        Discriminator = null
                    },
                    Status = new TradeRequestQueryStatus() {
                        Option = "online"
                    },
                    Stats = new List<TradeRequestQueryStat>() {
                        new TradeRequestQueryStat() {
                            Type = "and",
                            Filters = new List<TradeRequestQueryStatFilter>()
                        }
                    },
                    Filters = new TradeRequestQueryFilters() {
                        TradeFilters = new FiltersGroup<TradeFilters>() {
                            Filters = new TradeFilters() {
                                SaleType = new TradeFiltersOption() {
                                    Option = "priced"
                                }
                            }
                        }
                    }
                },
                Sort = new TradeRequestSort() {
                    Price = "asc"
                }
            };

            return body;
        }
    }
}
