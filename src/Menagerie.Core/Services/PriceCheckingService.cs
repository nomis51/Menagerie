using Menagerie.Core.Models;
using Menagerie.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class PriceCheckingService {
        #region Constructors
        public PriceCheckingService() { }
        #endregion

        #region Public methods
        public async Task<PriceCheckResult> PriceCheck(Item item) {
            var request = AppService.Instance.CreateTradeRequest(item);
            var searchResult = await AppService.Instance.GetTradeRequestResults(request);

            if (searchResult == null || searchResult.Result.Count == 0) {
                return null;
            }

            return AppService.Instance.GetTradeResults(searchResult, item);
        }
        #endregion
    }
}
