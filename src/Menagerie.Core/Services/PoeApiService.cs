using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Extensions;
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
using Menagerie.Core.Extensions;
using System.Threading;
using Menagerie.Core.Models.PoeApi;
using Menagerie.Core.Models.PoeApi.Stash;

namespace Menagerie.Core.Services {
    public class PoeApiService : IService {
        #region Constants

        private static readonly ILog log = LogManager.GetLogger(typeof(PoeApiService));
        private readonly Uri ALT_POE_API_BASE_URL = new Uri("http://api.pathofexile.com");
        private readonly Uri POE_API_BASE_URL = new Uri("https://www.pathofexile.com");
        private const string POE_API_LEAGUES = "leagues?compact=1";
        private const string POE_API_TRADE = "api/trade/search";
        private const string POE_API_FETCH = "api/trade/fetch";
        private const string POE_API_CHARS = "character-window/get-stash-items";
        private const int CACHE_EXPIRATION_TIME_MINS = 15;
        #endregion

        #region Members
        private HttpService _altHttpService;
        private HttpService _httpService;
        private HttpService _authHttpService;
        private ItemCache Cache;
        private StashTab ChaosRecipeTab;
        #endregion


        public PoeApiService() {
            log.Trace("Intializing PoeApiService");
            _altHttpService = new HttpService(ALT_POE_API_BASE_URL);
            _httpService = new HttpService(POE_API_BASE_URL);
        }

        public async Task<List<string>> GetLeagues() {
            log.Trace("Getting leagues");
            try {
                var response = _altHttpService.Client.GetAsync($"/{POE_API_LEAGUES}").Result;
                var result = await _altHttpService.ReadResponse<List<Dictionary<string, string>>>(response);

                return ParseLeagues(result);
            } catch (Exception e) {
                log.Error("Error while getting leagues ", e);
            }

            return new List<string>();
        }

        private async Task GetChaosRecipeStashTab() {
            var config = AppService.Instance.GetConfig();

            var response = await _authHttpService.Client.GetAsync($"/{POE_API_CHARS}?league={config.CurrentLeague}&tabs=0&tabIndex={config.ChaosRecipeTabIndex}&accountName={config.PlayerName}");
            ChaosRecipeTab = await _authHttpService.ReadResponse<StashTab>(response);

            var result = CalculateChaosRecipe(ChaosRecipeTab);
            AppService.Instance.NewChaosRecipeResult(result);
        }

        private void SetResult(string type, ref ChaosRecipeResult result) {
            switch (type) {
                case "Rings":
                    ++result.NbRings;
                    break;

                case "Belts":
                    ++result.NbBelts;
                    break;

                case "Amulets":
                    ++result.NbAmulets;
                    break;

                case "Armours/Helmets":
                    ++result.NbHelmets;
                    break;

                case "Armours/Boots":
                    ++result.NbBoots;
                    break;

                case "Armours/Gloves":
                    ++result.NbGloves;
                    break;

                case "Armours/BodyArmours":
                    ++result.NbBodyArmours;
                    break;

                case "Armours/Shields":
                    ++result.NbOffHands;
                    break;

                case "Weapons/OneHandWeapons":
                    ++result.Nb1HWeapons;
                    break;

                case "Weapons/TwoHandWeapons":
                    ++result.Nb2HWeapons;
                    break;

                default:
                    var idk = 0;
                    break;
            }
        }

        private ChaosRecipeResult CalculateChaosRecipe(StashTab tab) {
            ChaosRecipeResult result = new ChaosRecipeResult();

            string startStr = "https://web.poecdn.com/image/Art/2DItems/";

            foreach (var item in tab.Items) {
                if (item.FrameType == 2) {
                    int startIndex = item.IconUrl.IndexOf(startStr);

                    if (startIndex == -1) {
                        continue;
                    }

                    startIndex += startStr.Length;

                    int endIndex = item.IconUrl.IndexOf("/", startIndex);

                    if (endIndex == -1) {
                        continue;
                    }

                    string type = item.IconUrl.Substring(startIndex, endIndex - startIndex);

                    if (type == "Weapons") {
                        int nextIndex = item.IconUrl.IndexOf("/", endIndex + 1);

                        if (nextIndex != -1) {
                            type = item.IconUrl.Substring(startIndex, nextIndex - startIndex);
                        }
                    }

                    if (type == "Armours") {
                        int nextIndex = item.IconUrl.IndexOf("/", endIndex + 1);

                        if (nextIndex != -1) {
                            type = item.IconUrl.Substring(startIndex, nextIndex - startIndex);
                        }
                    }

                    SetResult(type, ref result);
                }
            }

            return result;
        }

        private List<string> ParseLeagues(List<Dictionary<string, string>> json) {
            log.Trace("Parsing leagues");
            return json.Select(l => l["id"])
                .ToList()
                .FindAll(n => n.IndexOf("SSF") == -1)
                .ToList();
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request, string league) {
            var json = _httpService.SerializeBody(request);

            var response = _httpService.Client.PostAsync($"/{POE_API_TRADE}/{league}", json).Result;

            SearchResult result = await _httpService.ReadResponse<SearchResult>(response);

            if (result == null || result.Error != null) {
                throw new Exception("Error while getting trade request results");
            }

            result.League = league;

            return result;
        }


        public PriceCheckResult GetTradeResults(SearchResult search, int nbResults = 10) {
            FetchResult result = new FetchResult() {
                Result = new List<FetchResultElement>()
            };
            List<Task> queries = new List<Task>();

            if (nbResults > 0 && nbResults <= 10) {
                result = GetTradeResults(search.Id, search.Result.Take(nbResults).ToList()).Result;
            } else {
                object _lock = new object();
                int i = 0;

                while (i < search.Result.Count) {
                    int k = i;
                    queries.Add(Task.Run(() => {
                        var r = GetTradeResults(search.Id, search.Result.Skip(k).Take(k + 10 < search.Result.Count ? 10 : search.Result.Count - k).ToList()).Result;

                        if (r != null) {
                            lock (_lock) {
                                result.Result.AddRange(r.Result);
                            }
                        }
                    }));

                    i += 10;
                    Thread.Sleep(50);
                }
            }

            if (queries.Any()) {
                Task.WaitAll(queries.ToArray());
            }

            if (result == null || result.Result == null) {
                return new PriceCheckResult() {
                    Results = new List<PricingResult>()
                };
            }

            return new PriceCheckResult() {
                Results = result.Result.Select(p => new PricingResult() {
                    ItemName = p.Item.Name,
                    ItemType = p.Item.Type,
                    Currency = p.Listing.Price.Currency,
                    Price = p.Listing.Price.Amount,
                    CurrencyImageLink = AppService.Instance.GetCurrencyImageLink(p.Listing.Price.Currency),
                    PlayerName = p.Listing.Account.Name
                }).ToList()
            };
        }

        private async Task<FetchResult> GetTradeResults(string queryId, List<string> resultIds) {
            var ids = string.Join(",", resultIds);
            var response = _httpService.Client.GetAsync($"/{POE_API_FETCH}/{ids}?query={queryId}").Result;

            FetchResult result = await _httpService.ReadResponse<FetchResult>(response);

            if (result == null) {
                throw new Exception("Error while getting trade results");
            }

            return result;
        }

        public TradeRequest CreateTradeRequest(Offer offer) {
            TradeRequest body = new TradeRequest() {
                Query = new TradeRequestQuery() {
                    Term = offer == null ? null : offer.ItemName,
                    Status = new TradeRequestQueryStatus() {
                        Option = "any"
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
                                },
                                Account = new TradeFiltersAccount() {
                                    Input = AppService.Instance.GetConfig().PlayerName
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

        public PriceCheckResult VerifyScam(Offer offer) {
            if (Cache == null || Cache.Items == null || Cache.Items.League != offer.League) {
                return null;
            }

            bool foundItem = false;
            List<PricingResult> founds = new List<PricingResult>();

            foreach (var r in Cache.Items.Results) {
                bool emptyName = string.IsNullOrEmpty(r.ItemName);
                if (((!emptyName && r.ItemName == offer.ItemName) || (emptyName && r.ItemType == offer.ItemName))) {
                    foundItem = true;
                    founds.Add(r);

                    if (r.Price == offer.Price && r.Currency == offer.Currency) {
                        return null;
                    }
                }
            }

            return foundItem ? new PriceCheckResult() {
                Results = founds,
                League = Cache.Items.League
            } : null;
        }

        public void UpdateCacheItemsCache() {
            Cache.Items = AppService.Instance.PriceCheck(null, 0).Result;
            Cache.Items.League = AppService.Instance.GetConfig().CurrentLeague;
        }

        private void AutoUpdateItemsCache() {
            log.Trace("Starting auto cache update");

            Cache = new ItemCache() {
                Items = new PriceCheckResult()
            };

            while (true) {
                try {
                    UpdateCacheItemsCache();
                } catch (Exception e) {
                    log.Trace("Error while update items cache ", e);
                }

                Thread.Sleep(CACHE_EXPIRATION_TIME_MINS * 60 * 1000);
            }
        }

        private void AutoUpdateChaosRecipeTab() {
            var config = AppService.Instance.GetConfig();

            while (true) {
                try {
                    GetChaosRecipeStashTab().Wait();
                } catch (Exception e) {
                    log.Trace("Error while updating chaos recipe tab", e);
                }

                Thread.Sleep(config.ChaosRecipeRefreshRate * 60 * 1000);
            }
        }

        public void Start() {
            log.Trace("Starting PoeApiService");
            _authHttpService = new HttpService(POE_API_BASE_URL, new List<Cookie>() { new Cookie("POESESSID", AppService.Instance.GetConfig().POESESSID) });

            Task.Run(() => AutoUpdateItemsCache());

            Task.Run(() => {
                Thread.Sleep(3000);
                AutoUpdateChaosRecipeTab();
            });
        }
    }
}
