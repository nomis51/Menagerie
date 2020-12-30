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

namespace Menagerie.Core.Services {
    public class PoeApiService : IService {
        #region Constants
        private const int NB_RESULT_PER_QUERY = 10;
        private readonly Uri ALT_POE_API_BASE_URL = new Uri("http://api.pathofexile.com");
        private readonly Uri POE_API_BASE_URL = new Uri("https://www.pathofexile.com");
        private const string POE_API_LEAGUES = "leagues?compact=1";
        private const string POE_API_TRADE = "api/trade/search";
        private const string POE_API_FETCH = "api/trade/fetch";
        #endregion

        #region Members
        private HttpService _httpService;
        private HttpService _altHttpService;
        #endregion


        public PoeApiService() {
            _httpService = new HttpService(POE_API_BASE_URL);
            _altHttpService = new HttpService(ALT_POE_API_BASE_URL);
        }

        public async Task<List<string>> GetLeagues() {
            var response = _altHttpService.Client.GetAsync($"/{POE_API_LEAGUES}").Result;
            var result = await _altHttpService.ReadResponse<List<Dictionary<string, string>>>(response);

            return ParseLeagues(result);
        }

        private List<string> ParseLeagues(List<Dictionary<string, string>> json) {
            return json.Select(l => l["id"])
                .ToList()
                .FindAll(n => n.IndexOf("SSF") == -1)
                .ToList();
        }

       

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request) {
            var json = _httpService.SerializeBody(request);

            var response = _httpService.Client.PostAsync($"/{POE_API_TRADE}/{AppService.Instance.GetConfig().CurrentLeague}", json).Result;

            SearchResult result = await _httpService.ReadResponse<SearchResult>(response);

            if (result == null || result.Error != null) {
                throw new Exception("Error while getting trade request results");
            }

            return result;
        }

        public PriceCheckResult GetTradeResults(SearchResult search, Item item, int nbResults = 20) {
            List<FetchResult> results = new List<FetchResult>();
            object locker = new object();

            var loopResult = Parallel.ForEach(SteppedIterator.GetIterator(0, nbResults, NB_RESULT_PER_QUERY),  (i) => {
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

            return AppService.Instance.CalculateChaosValues(new PriceCheckResult() {
                Item = item,
                Results = results[0].Result.Select(p => new PricingResult() {
                    Currency = p.Listing.Price.Currency,
                    Price = p.Listing.Price.Amount,
                    CurrencyImageLink = AppService.Instance.GetCurrencyImageLink(p.Listing.Price.Currency),
                    PlayerName = p.Listing.Account.Name
                }).ToList()
            });
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

        public void Start() {
        }
    }
}
