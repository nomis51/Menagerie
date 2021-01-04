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

namespace Menagerie.Core.Services {
    public class PoeApiService : IService {
        #region Constants

        private static readonly ILog log = LogManager.GetLogger(typeof(PoeApiService));
        private readonly Uri ALT_POE_API_BASE_URL = new Uri("http://api.pathofexile.com");
        private readonly Uri POE_API_BASE_URL = new Uri("https://www.pathofexile.com");
        private const string POE_API_LEAGUES = "leagues?compact=1";
        private const string POE_API_TRADE = "api/trade/search";
        private const string POE_API_FETCH = "api/trade/fetch";
        #endregion

        #region Members
        private HttpService _altHttpService;
        private HttpService _httpService;
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

            return result;
        }

        public PriceCheckResult GetTradeResults(SearchResult search) {
            var result = GetTradeResults(search.Id, search.Result.Take(10).ToList()).Result;

            return new PriceCheckResult() {
                Results = result.Result.Select(p => new PricingResult() {
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
                    Term = offer.ItemName,
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

        public void Start() {
            log.Trace("Starting PoeApiService");
        }
    }
}
