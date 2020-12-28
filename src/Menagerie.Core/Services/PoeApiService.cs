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

        private const int NB_RESULT_PER_QUERY = 10;
        private const string POE_API_LEAGUES = "leagues?compact=1";
        private const string POE_API_TRADE = "api/trade/search";
        private const string POE_API_FETCH = "api/trade/fetch";

        private static readonly HttpClient Client = new HttpClient();

        private PoeApiService() {
            SetupClient();
        }

        private void SetupClient() {
            Client.BaseAddress = new Uri("https://www.pathofexile.com");
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");
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
            return new StringContent(
                JsonConvert.SerializeObject(obj, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                Encoding.UTF8,
                "application/json"
            );
        }

        public List<string> GetLeagues() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(POE_API_LEAGUES);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            string data = "";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream)) {
                data = reader.ReadToEnd();
            }

            return ParseLeagues(data);
        }

        private List<string> ParseLeagues(string data) {
            var json = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(data);
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

        public FetchResult GetTradeResults(SearchResult search, int nbResults = 20) {
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

            if (results.Count == 1) {
                return results[0];
            }

            if (results.Count == 0) {
                return null;
            }

            for (int i = 1; i < results.Count; ++i) {
                results[0].Result.AddRange(results[i].Result);
            }

            return results[0];
        }

        private async Task<FetchResult> GetTradeResults(string queryId, List<string> resultIds) {
            var ids = string.Join(",", resultIds);
            var response = Client.GetAsync($"/{POE_API_FETCH}/{ids}?query={queryId}").Result;

            FetchResult result = await ReadResponse<FetchResult>(response);

            if (result == null ) {
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
