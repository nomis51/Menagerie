using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class PriceCheckingService : IService {
        #region Constructors
        public PriceCheckingService() { }
        #endregion

        #region Public methods
        public async Task<PriceCheckResult> PriceCheck(Offer offer) {
            var request = AppService.Instance.CreateTradeRequest(offer);
            var searchResult = await AppService.Instance.GetTradeRequestResults(request, offer.League);

            if (searchResult == null || searchResult.Result.Count == 0) {
                return null;
            }

            return AppService.Instance.GetTradeResults(searchResult);
        }

        public void Start() {  }
        #endregion
    }
}
