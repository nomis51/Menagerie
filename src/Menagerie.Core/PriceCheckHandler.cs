using Menagerie.Core.Models;
using Menagerie.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core {
    public class PriceCheckHandler {
        #region Singleton
        private static PriceCheckHandler _instance;
        public static PriceCheckHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new PriceCheckHandler();
                }

                return _instance;
            }
        }
        #endregion

        private PriceCheckHandler() { }

        public async Task<PriceCheckResult> PriceCheck(Item item) {
            var request = PoeApiService.Instance.CreateTradeRequest(item);
            var searchResult = await PoeApiService.Instance.GetTradeRequestResults(request);
            
            if(searchResult == null || searchResult.Result.Count == 0) {
                return null;
            }

           return await PoeApiService.Instance.GetTradeResults(searchResult, item);
        }
    }
}
