using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class HttpService : Service {
        #region Props
        public HttpClient Client { get; private set; }
        #endregion

        #region Constructors
        public HttpService(Uri baseUrl) {
            SetupClient(baseUrl);
        }
        #endregion

        #region Private methods
        private void SetupClient(Uri baseUrl) {
            Client.BaseAddress = baseUrl;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Powered-By", "Menagerie");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Menagerie");
        }
        #endregion

        #region Public methods
        public async Task<T> ReadResponse<T>(HttpResponseMessage response) {
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
        #endregion
    }
}
