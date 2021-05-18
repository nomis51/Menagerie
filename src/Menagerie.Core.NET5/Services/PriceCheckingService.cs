using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class PriceCheckingService : IService {
        #region Constructors
        public PriceCheckingService() { }
        #endregion

        #region Public methods
        public async Task<PriceCheckResult> PriceCheck(Offer offer, int nbResults = 10) {
            var request = AppService.Instance.CreateTradeRequest(offer);
            var searchResult = await AppService.Instance.GetTradeRequestResults(request, offer == null ? AppService.Instance.GetConfig().CurrentLeague : offer.League);

            if (searchResult == null || searchResult.Result.Count == 0) {
                return null;
            }

            var result = AppService.Instance.GetTradeResults(searchResult, nbResults);

            return result;
        }

        public void Start() { }
        #endregion
    }
}
