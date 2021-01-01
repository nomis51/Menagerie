using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using Menagerie.Core.Extensions;
using Menagerie.Core.Models;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace Menagerie.Core.Services {
    public class TradeService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(TradeService));
        private const int OFFER_EXPIRATION_MINS = 15;
        #endregion

        #region Members
        Dictionary<string, Offer> Offers = new Dictionary<string, Offer>();

        #endregion

        #region Constructors
        public TradeService() {
            log.Trace("Initializing TradeService");
        }
        #endregion

        #region Private methods
        private void AutoCleanOffers() {
            log.Trace("Auto cleaning offers");
            while (true) {
                log.Trace("Cleaning offers");
                for (int i = 0; i < Offers.Count; ++i) {
                    if ((DateTime.Now - Offers.ElementAt(i).Value.Time).TotalMinutes >= OFFER_EXPIRATION_MINS) {
                        Offers.Remove(Offers.ElementAt(i).Key);
                        --i;
                    }
                }

                Thread.Sleep(OFFER_EXPIRATION_MINS * 60 * 1000);
            }
        }
        #endregion

        #region Public methods
        public bool IsAlreadySold(Offer offer) {
            if (!offer.IsOutgoing && Offers.ContainsKey(offer.ItemName)) {
                return Offers[offer.ItemName].Currency == offer.Currency &&
                    Offers[offer.ItemName].Price == offer.Price &&
                    Offers[offer.ItemName].League == offer.League;
            }

            return false;
        }

        public void AddSoldOffer(Offer offer) {
            Offers.Add(offer.ItemName, offer);
        }

        public void SaveTrade(Offer offer) {
            AppService.Instance.SaveTrade(offer);
        }

        public void Start() {
            log.Trace("Starting TradeService");
            Task.Run(() => AutoCleanOffers());
        }
        #endregion
    }
}
