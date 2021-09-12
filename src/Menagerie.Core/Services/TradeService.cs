using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using Menagerie.Core.Extensions;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Menagerie.Core.Models.ItemsScan;
using Menagerie.Core.Models.Parsing;
using Menagerie.Core.Models.Trades;

namespace Menagerie.Core.Services
{
    public class TradeService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(TradeService));
        private const int OfferExpirationMinutes = 15;
        private readonly Size _tradeWindowSquareSize = new(53, 53);
        private readonly Point _tradeWindowTopCornerPosition = new(312, 202);
        private readonly Size _tradeWindowColsRowsSize = new(12, 5);
        private List<Point> _tradeWindowSquarePositions = new();

        #endregion

        #region Members

        private readonly List<Offer> _offers = new List<Offer>();

        #endregion

        #region Constructors

        public TradeService()
        {
            Log.Trace("Initializing TradeService");
            CalculateTradeWindowSquarePositions();
        }

        #endregion

        #region Private methods

        private void CalculateTradeWindowSquarePositions()
        {
            var halfSquareHeight = _tradeWindowSquareSize.Height / 2;
            var halfSquareWidth = _tradeWindowSquareSize.Width / 2;

            for (var col = 0; col < _tradeWindowColsRowsSize.Width; ++col)
            {
                for (var row = 0; row < _tradeWindowColsRowsSize.Height; ++row)
                {
                    _tradeWindowSquarePositions.Add(
                        new Point(
                            _tradeWindowTopCornerPosition.X + (col * (_tradeWindowSquareSize.Width)) + halfSquareWidth,
                            _tradeWindowTopCornerPosition.Y + (row * (_tradeWindowSquareSize.Height)) + halfSquareHeight
                        )
                    );
                }
            }
        }

        private void AutoCleanOffers()
        {
            Log.Trace("Auto cleaning offers");
            while (true)
            {
                Log.Trace("Cleaning offers");
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

        public double ValidateTradeWindow(Price expectedPrice)
        {
            var chaosValue = 0.0d;
            var items = new List<TradeWindowItem>();

            while (AppService.Instance.GetClipboardValue() != "$temp")
            {
                AppService.Instance.SetClipboard("$temp");
            }

            foreach (var position in _tradeWindowSquarePositions)
            {
                AppService.Instance.MoveMouse(position.X, position.Y);

                Thread.Sleep(20);

                var value = "$temp";
                var i = 0;

                do
                {
                    AppService.Instance.SendCtrlC();
                    value = AppService.Instance.GetClipboardValue();
                    ++i;
                } while (value == "$temp" && i < 3);

                var item = string.IsNullOrEmpty(value) || value == "$temp" ? null : AppService.Instance.ParseTradeWindowItem(value);
                items.Add(item);

                if (item != null)
                {
                    chaosValue += item.StackSize * (item.Name == "Chaos Orb" ? 1 : AppService.Instance.GetChaosValueOfCurrency(item.Name));
                }

                while (AppService.Instance.GetClipboardValue() != "$temp")
                {
                    AppService.Instance.SetClipboard("$temp");
                }
            }

            return chaosValue;
        }

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
            Log.Trace("Starting TradeService");
            Task.Run(AutoCleanOffers);
        }

        #endregion
    }
}