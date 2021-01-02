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
        private const string POE_API_LEAGUES = "leagues?compact=1";
        #endregion

        #region Members
        private HttpService _altHttpService;
        #endregion


        public PoeApiService() {
            log.Trace("Intializing PoeApiService");
            _altHttpService = new HttpService(ALT_POE_API_BASE_URL);
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

        public void Start() {
            log.Trace("Starting PoeApiService");
        }
    }
}
