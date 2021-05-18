using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;
using System.Collections.Generic;
using System.Net;

namespace Menagerie.Core.Services {
    public class HttpService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(HttpService));
        #endregion

        #region Props
        public HttpClient Client { get; private set; }
        #endregion

        #region Constructors
        public HttpService(Uri baseUrl) {
            log.Trace("Initializing HttpService");
            Client = new HttpClient();
            SetupClient(baseUrl);
        }

        public HttpService(Uri baseUrl, List<Cookie> cookies) {
            log.Trace("Initializing HttpService");
            Client = new HttpClient(GetHandler(baseUrl, cookies));
            SetupClient(baseUrl);
        }
        #endregion

        #region Private methods
        private HttpClientHandler GetHandler(Uri baseUrl, List<Cookie> cookies) {
            var cookieContainer = new CookieContainer();

            cookies.ForEach(c => cookieContainer.Add(baseUrl, c));

            return new HttpClientHandler() { CookieContainer = cookieContainer };
        }

        private void SetupClient(Uri baseUrl) {
            log.Trace($"Settings client for {baseUrl}");
            Client.BaseAddress = baseUrl;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");
        }
        #endregion

        #region Public methods
        public async Task<T> ReadResponse<T>(HttpResponseMessage response) {
            log.Trace($"Reading response {response.StatusCode} {response.ReasonPhrase}");
            if (response.IsSuccessStatusCode) {
                var content = await response.Content.ReadAsStringAsync();
                try {
                    return JsonConvert.DeserializeObject<T>(content, new JsonSerializerSettings() {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    });
                } catch (Exception e) {
                    log.Error("Error while reading response", e);
                }
            } else {
                log.Error($"Errored response content: \n{response.Content.ReadAsStringAsync().Result}");
            }

            return default(T);
        }

        public HttpContent SerializeBody<T>(T obj) {
            log.Trace($"Serializing body content for {typeof(T)}");
            try {
                var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

                return new StringContent(json, Encoding.UTF8, "application/json");
            } catch (Exception e) {
                log.Error($"Serializing body content for {typeof(T)}", e);
            }

            return null;
        }
        #endregion
    }
}
