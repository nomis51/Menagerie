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
        List<Offer> Offers = new List<Offer>();

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
                    if ((DateTime.Now - Offers.ElementAt(i).Time).TotalMinutes >= OFFER_EXPIRATION_MINS) {
                        Offers.RemoveAt(i);
                        --i;
                    }
                }

                Thread.Sleep(OFFER_EXPIRATION_MINS * 60 * 1000);
            }
        }
        #endregion

        #region Public methods
        public bool IsAlreadySold(Offer offer) {
            if (!offer.IsOutgoing) {
                return Offers.FindIndex(o => o.ItemName == offer.ItemName && o.Price == offer.Price && o.Currency == offer.Currency && o.League == offer.League) != -1;
            }

            return false;
        }

        public void AddSoldOffer(Offer offer) {
            Offers.Add( offer);
            SaveTrade(offer);
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
