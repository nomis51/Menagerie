using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Extensions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Menagerie.Core.Models.PoeApi;
using Menagerie.Core.Models.PoeApi.Stash;
using Menagerie.Core.Models.Trades;

namespace Menagerie.Core.Services
{
    public class PoeApiService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(PoeApiService));
        private static readonly object LockStashApi = new object();
        private readonly Uri _altPoeApiBaseUrl = new Uri("http://api.pathofexile.com");
        private readonly Uri _poeApiBaseUrl = new Uri("https://www.pathofexile.com");
        private const string PoeApiLeagues = "leagues?compact=1";
        private const string PoeApiTrade = "api/trade/search";
        private const string PoeApiFetch = "api/trade/fetch";
        private const string PoeApiChars = "character-window/get-stash-items";
        private const int CacheExpirationTimeMinutes = 15;

        #endregion

        #region Members

        private readonly HttpService _altHttpService;
        private readonly HttpService _httpService;
        private HttpService _authHttpService;
        private ItemCache _cache;
        private StashTab _chaosRecipeTab;
        private bool _stashApiUpdated = true;

        #endregion


        public PoeApiService()
        {
            Log.Trace("Initializing PoeApiService");
            _altHttpService = new HttpService(_altPoeApiBaseUrl);
            _httpService = new HttpService(_poeApiBaseUrl);
        }

        public async Task<List<string>> GetLeagues()
        {
            Log.Trace("Getting leagues");
            try
            {
                var response = _altHttpService.Client.GetAsync($"/{PoeApiLeagues}").Result;
                var result = await HttpService.ReadResponse<List<Dictionary<string, string>>>(response);

                return ParseLeagues(result);
            }
            catch (Exception e)
            {
                Log.Error("Error while getting leagues ", e);
            }

            return new List<string>();
        }

        public void StashApiReady()
        {
            lock (LockStashApi)
            {
                _stashApiUpdated = true;
            }
        }

        private async Task GetChaosRecipeStashTab()
        {
            if (_authHttpService != null)
            {
                var config = AppService.Instance.GetConfig();

                var response = await _authHttpService.Client.GetAsync(
                    $"/{PoeApiChars}?league={config.CurrentLeague}&tabs=0&tabIndex={config.ChaosRecipeTabIndex}&accountName={config.PlayerName}");
                _chaosRecipeTab = await HttpService.ReadResponse<StashTab>(response);

                var result = CalculateChaosRecipe(_chaosRecipeTab);
                AppService.Instance.NewChaosRecipeResult(result);
            }
        }

        private void SetResult(string type, ref ChaosRecipeResult result)
        {
            switch (type)
            {
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
            }
        }

        private ChaosRecipeResult CalculateChaosRecipe(StashTab tab)
        {
            var result = new ChaosRecipeResult();

            const string startStr = "https://web.poecdn.com/image/Art/2DItems/";

            foreach (var item in tab.Items)
            {
                if (item.FrameType != 2 || item.ItemLevel is < 60 or >= 75) continue;
                var startIndex = item.IconUrl.IndexOf(startStr, StringComparison.Ordinal);

                if (startIndex == -1)
                {
                    continue;
                }

                startIndex += startStr.Length;

                var endIndex = item.IconUrl.IndexOf("/", startIndex, StringComparison.Ordinal);

                if (endIndex == -1)
                {
                    continue;
                }

                var type = item.IconUrl.Substring(startIndex, endIndex - startIndex);

                if (type == "Weapons")
                {
                    var nextIndex = item.IconUrl.IndexOf("/", endIndex + 1, StringComparison.Ordinal);

                    if (nextIndex != -1)
                    {
                        type = item.IconUrl.Substring(startIndex, nextIndex - startIndex);
                    }
                }

                if (type == "Armours")
                {
                    var nextIndex = item.IconUrl.IndexOf("/", endIndex + 1, StringComparison.Ordinal);

                    if (nextIndex != -1)
                    {
                        type = item.IconUrl.Substring(startIndex, nextIndex - startIndex);
                    }
                }

                SetResult(type, ref result);
            }

            return result;
        }

        private static List<string> ParseLeagues(IEnumerable<Dictionary<string, string>> json)
        {
            Log.Trace("Parsing leagues");
            return json.Select(l => l["id"])
                .ToList()
                .FindAll(n => !n.Contains("SSF"))
                .ToList();
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request, string league)
        {
            var json = HttpService.SerializeBody(request);

            var response = _httpService.Client.PostAsync($"/{PoeApiTrade}/{league}", json).Result;

            var result = await HttpService.ReadResponse<SearchResult>(response);

            if (result is not {Error: null})
            {
                Log.Error("Error while getting trade request results");
                return null;
            }

            result.League = league;

            return result;
        }


        public PriceCheckResult GetTradeResults(SearchResult search, int nbResults = 10)
        {
            var result = new FetchResult()
            {
                Result = new List<FetchResultElement>()
            };
            var queries = new List<Task>();

            if (nbResults is > 0 and <= 10)
            {
                result = GetTradeResults(search.Id, search.Result.Take(nbResults).ToList()).Result;
            }
            else
            {
                var resultLock = new object();
                var i = 0;

                while (i < search.Result.Count)
                {
                    var k = i;
                    queries.Add(Task.Run(() =>
                    {
                        var r = GetTradeResults(search.Id,
                            search.Result.Skip(k).Take(k + 10 < search.Result.Count ? 10 : search.Result.Count - k)
                                .ToList()).Result;

                        if (r == null) return;
                        lock (resultLock)
                        {
                            result.Result.AddRange(r.Result);
                        }
                    }));

                    i += 10;
                    Thread.Sleep(50);
                }
            }

            if (queries.Any())
            {
                Task.WaitAll(queries.ToArray());
            }

            if (result?.Result == null)
            {
                return new PriceCheckResult()
                {
                    Results = new List<PricingResult>()
                };
            }

            return new PriceCheckResult()
            {
                Results = result.Result.Select(p => new PricingResult()
                {
                    ItemName = p.Item.Name,
                    ItemType = p.Item.Type,
                    Currency = p.Listing.Price.Currency,
                    Price = p.Listing.Price.Amount,
                    CurrencyImageLink = AppService.Instance.GetCurrencyImageLink(p.Listing.Price.Currency),
                    PlayerName = p.Listing.Account.Name
                }).ToList()
            };
        }

        private async Task<FetchResult> GetTradeResults(string queryId, IEnumerable<string> resultIds)
        {
            var ids = string.Join(",", resultIds);
            var response = _httpService.Client.GetAsync($"/{PoeApiFetch}/{ids}?query={queryId}").Result;

            var result = await HttpService.ReadResponse<FetchResult>(response);

            if (result != null) return result;
            Log.Error("Error while getting trade results");
            return null;
        }

        public static TradeRequest CreateTradeRequest(Offer offer)
        {
            var body = new TradeRequest()
            {
                Query = new TradeRequestQuery()
                {
                    Term = offer?.ItemName,
                    Status = new TradeRequestQueryStatus()
                    {
                        Option = "any"
                    },
                    Stats = new List<TradeRequestQueryStat>()
                    {
                        new TradeRequestQueryStat()
                        {
                            Type = "and",
                            Filters = new List<TradeRequestQueryStatFilter>()
                        }
                    },
                    Filters = new TradeRequestQueryFilters()
                    {
                        TradeFilters = new FiltersGroup<TradeFilters>()
                        {
                            Filters = new TradeFilters()
                            {
                                SaleType = new TradeFiltersOption()
                                {
                                    Option = "priced"
                                },
                                Account = new TradeFiltersAccount()
                                {
                                    Input = AppService.Instance.GetConfig().PlayerName
                                }
                            }
                        }
                    }
                },
                Sort = new TradeRequestSort()
                {
                    Price = "asc"
                }
            };

            return body;
        }

        public PriceCheckResult VerifyScam(Offer offer)
        {
            if (_cache?.Items == null || _cache.Items.League != offer.League)
            {
                return null;
            }

            var foundItem = false;
            var founds = new List<PricingResult>();

            foreach (var r in from r in _cache.Items.Results
                let emptyName = string.IsNullOrEmpty(r.ItemName)
                where ((!emptyName && r.ItemName == offer.ItemName) || (emptyName && r.ItemType == offer.ItemName))
                select r)
            {
                foundItem = true;
                founds.Add(r);

                if (Math.Abs(r.Price - offer.Price) < 0.1 && r.Currency == offer.Currency)
                {
                    return null;
                }
            }

            return foundItem
                ? new PriceCheckResult()
                {
                    Results = founds,
                    League = _cache.Items.League
                }
                : null;
        }

        public void UpdateCacheItemsCache()
        {
            _cache.Items = AppService.Instance.PriceCheck(null, 0).Result;

            if (_cache.Items != null)
            {
                _cache.Items.League = AppService.Instance.GetConfig().CurrentLeague;
            }
        }

        private void AutoUpdateItemsCache()
        {
            Log.Trace("Starting auto cache update");

            _cache = new ItemCache()
            {
                Items = new PriceCheckResult()
            };

            while (true)
            {
                try
                {
                    UpdateCacheItemsCache();
                }
                catch (Exception e)
                {
                    Log.Trace("Error while update items cache ", e);
                }

                Thread.Sleep(CacheExpirationTimeMinutes * 60 * 1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void AutoUpdateChaosRecipeTab()
        {
            while (true)
            {
                var config = AppService.Instance.GetConfig();

                if (config.ChaosRecipeEnabled)
                {
                    if (_stashApiUpdated)
                    {
                        lock (LockStashApi)
                        {
                            _stashApiUpdated = false;
                        }

                        try
                        {
                            GetChaosRecipeStashTab().Wait();
                        }
                        catch (Exception e)
                        {
                            Log.Trace("Error while updating chaos recipe tab", e);
                        }

                        Thread.Sleep(config.ChaosRecipeRefreshRate * 60 * 1000);
                    }
                    else
                    {
                        Thread.Sleep(1 * 1000);
                    }
                }
                else
                {
                    Thread.Sleep(10 * 1000);
                }
            }
            // ReSharper disable once FunctionNeverReturns
        }

        public void Start()
        {
            Log.Trace("Starting PoeApiService");

            var config = AppService.Instance.GetConfig();

            if (config != null && !string.IsNullOrEmpty(config.POESESSID))
            {
                _authHttpService = new HttpService(_poeApiBaseUrl,
                    new List<Cookie>() {new Cookie("POESESSID", config.POESESSID)});
            }

            Task.Run(AutoUpdateItemsCache);

            if (AppService.Instance.GetConfig().ChaosRecipeEnabled)
            {
                Task.Run(() =>
                {
                    Thread.Sleep(3000);
                    AutoUpdateChaosRecipeTab();
                });
            }
        }
    }
}