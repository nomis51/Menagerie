﻿using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Menagerie.Core.Models.Trades;
using Serilog;

namespace Menagerie.Core.Services
{
    public class TradeService : IService
    {
        #region Constants

        private const int OfferExpirationMinutes = 15;

        #endregion

        #region Members

        private readonly List<Offer> _offers = new List<Offer>();

        #endregion

        #region Constructors

        public TradeService()
        {
            Log.Information("Initializing TradeService");
        }

        #endregion

        #region Private methods

        private void AutoCleanOffers()
        {
            Log.Information("Auto cleaning offers");
            while (true)
            {
                Log.Information("Cleaning offers");
                for (var i = 0; i < _offers.Count; ++i)
                {
                    if (!((DateTime.Now - _offers.ElementAt(i).Time).TotalMinutes >= OfferExpirationMinutes)) continue;
                    _offers.RemoveAt(i);
                    --i;
                }

                Thread.Sleep(OfferExpirationMinutes * 60 * 1000);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        #endregion

        #region Public methods

        public bool IsAlreadySold(Offer offer)
        {
            if (!offer.IsOutgoing)
            {
                return _offers.FindIndex(o =>
                    o.ItemName == offer.ItemName && Math.Abs(o.Price - offer.Price) < 0.1 &&
                    o.Currency == offer.Currency && o.League == offer.League) != -1;
            }

            return false;
        }

        public void AddSoldOffer(Offer offer)
        {
            _offers.Add(offer);
            SaveTrade(offer);
        }

        private static void SaveTrade(Offer offer)
        {
            AppService.Instance.SaveTrade(offer);
        }

        public void Start()
        {
            Log.Information("Starting TradeService");
            Task.Run(AutoCleanOffers);
        }

        #endregion
    }
}