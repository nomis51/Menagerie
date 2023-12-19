using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class TradesStatisticsViewModel : ReactiveObject
{
    #region Members

    private TradeStatsDto _tradeStats = new();

    #endregion

    #region Props

    public TradeStatsDto TradesStats
    {
        get => _tradeStats;
        set => this.RaiseAndSetIfChanged(ref _tradeStats, value);
    }

    public SeriesCollection CurrencySeriesCollection
    {
        get
        {
            SeriesCollection series = new();

            foreach (var key in TradesStats.CurrencyTypeToAmount.Keys)
            {
                series.Add(new PieSeries
                {
                    Title = key,
                    Values = new ChartValues<double> { TradesStats.CurrencyTypeToAmount[key] },
                    DataLabels = true
                });
            }

            return series;
        }
    }

    public SeriesCollection ItemTypesSeriesCollection
    {
        get
        {
            SeriesCollection series = new();

            foreach (var key in TradesStats.ItemTypeToAmount.Keys)
            {
                series.Add(new PieSeries
                {
                    Title = key,
                    Values = new ChartValues<double> { TradesStats.ItemTypeToAmount[key] },
                    DataLabels = true
                });
            }

            return series;
        }
    }

    public SeriesCollection TradesSeriesCollection =>
        new()
        {
            new LineSeries
            {
                Title = "Trades",
                Values = new ChartValues<int>(TradesStats.DateToNbTrades.Values)
            }
        };

    public string[] TradesLabels => TradesStats.DateToNbTrades.Keys.ToArray();

    public SeriesCollection ChaosValuesSeriesCollection => new SeriesCollection
    {
        new LineSeries
        {
            Title = "Chaos orb value",
            Values = new ChartValues<double>(TradesStats.DateToChaosValue.Values)
        }
    };

    public string[] ChaosValuesLabels => TradesStats.DateToChaosValue.Keys.ToArray();

    public string NbTradesTodayStr => $"{TradesStats.NbTradesToday} trade{(TradesStats.NbTradesToday > 1 ? "s" : "")}";

    public string ChaosValueTodayStr =>
        $"{TradesStats.ChaosValueToday} chaos orb{(TradesStats.ChaosValueToday > 1 ? "s" : "")}";

    public string ExaltedValueTodayStr =>
        $"{TradesStats.ExaltedValueToday} exalted orb{(TradesStats.ExaltedValueToday > 1 ? "s" : "")}";

    #endregion

    #region Constructors

    public TradesStatisticsViewModel()
    {
    }

    #endregion

    #region Public methods

    public void LoadStats()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            TradesStats = AppService.Instance.GetTradesStatistics();

            this.RaisePropertyChanged(nameof(ExaltedValueTodayStr));
            this.RaisePropertyChanged(nameof(ChaosValueTodayStr));
            this.RaisePropertyChanged(nameof(NbTradesTodayStr));
            this.RaisePropertyChanged(nameof(ChaosValuesLabels));
            this.RaisePropertyChanged(nameof(ChaosValuesSeriesCollection));
            this.RaisePropertyChanged(nameof(TradesLabels));
            this.RaisePropertyChanged(nameof(TradesSeriesCollection));
            this.RaisePropertyChanged(nameof(ItemTypesSeriesCollection));
            this.RaisePropertyChanged(nameof(CurrencySeriesCollection));
        });
    }

    #endregion

    #region Private methods

    #endregion
}