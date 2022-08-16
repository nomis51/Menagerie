using LiteDB;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Services;

public class StatisticsService : IService
{
    #region Constants

    private const string TradesCollection = "trades";

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public void WriteIncomingTradeStatistic(IncomingOffer offer)
    {
        DatabaseProvider.Insert(TradesCollection, offer);
    }

    public void WriteIncomingTradeStatistic(OutgoingOffer offer)
    {
        DatabaseProvider.Insert(TradesCollection, offer);
    }

    public TradeStats CalculateStats()
    {
        try
        {
            TradeStats tradeStats = new();
            var query = DatabaseProvider.Query<IncomingOffer>(TradesCollection);
            if (query is null) return tradeStats;

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todayMinusSevenDays = today.AddDays(-7);

            var allTrades = query.FindAll().ToList();
            var todaysTrades = allTrades.Where(d => d.Time >= today && d.Time < tomorrow);

            foreach (var trade in todaysTrades)
            {
                ++tradeStats.NbTradesToday;
                tradeStats.ChaosValueToday += AppDataService.Instance.GetChaosValueOf(trade.Price, trade.Currency);
            }

            tradeStats.ExaltedValueToday = AppDataService.Instance.GetExaltedValue(tradeStats.ChaosValueToday);

            var lastWeekTrades = allTrades.Where(d => d.Time >= todayMinusSevenDays);

            foreach (var trade in lastWeekTrades)
            {
                var date = trade.Time.ToString("d MMMM");

                if (!tradeStats.DateToNbTrades.ContainsKey(date))
                {
                    tradeStats.DateToNbTrades.Add(date, 0);
                }

                ++tradeStats.DateToNbTrades[date];

                if (!tradeStats.DateToChaosValue.ContainsKey(date))
                {
                    tradeStats.DateToChaosValue.Add(date, 0d);
                }

                tradeStats.DateToChaosValue[date] += AppDataService.Instance.GetChaosValueOf(trade.Price, trade.Currency);

                var currency = CurrencyHelper.GetRealName(trade.Currency);

                if (!tradeStats.CurrencyTypeToAmount.ContainsKey(currency))
                {
                    tradeStats.CurrencyTypeToAmount.Add(currency, 0);
                }

                ++tradeStats.CurrencyTypeToAmount[currency];
            }

            return tradeStats;
        }
        catch (Exception)
        {
            // ignored
        }

        return new TradeStats();
    }

    #endregion

    #region Private methods

    #endregion
}