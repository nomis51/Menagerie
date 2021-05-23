using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Menagerie.Core.Extensions;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Menagerie.Core.Services;
using Menagerie.Core.Models;
using System.Windows;
using StatsOffer = Menagerie.Models.StatsOffer;

namespace Menagerie.ViewModels
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        #region Updater

        private ICommand mUpdater;

        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set { mUpdater = value; }
        }

        private class Updater : ICommand
        {
            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
            }

            #endregion
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private readonly static ILog log = LogManager.GetLogger(typeof(StatsViewModel));

        public SeriesCollection Trades { get; set; } = new SeriesCollection()
        {
            new LineSeries()
            {
                Title = "Amount of Trades",
                AreaLimit = 0,
                Values = new ChartValues<ObservableValue>()
            }
        };

        public SeriesCollection Currencies { get; set; } = new SeriesCollection()
        {
            new LineSeries()
            {
                Title = "Amount of Currency (in Chaos Orbs)",
                AreaLimit = 0,
                Values = new ChartValues<ObservableValue>()
            }
        };

        public SeriesCollection CurrencyGroups
        {
            get => _currencyGroups;
            set => _currencyGroups = value;
        }

        public List<StatsOffer> Offers { get; set; } = new();

        public List<string> Labels
        {
            get => _labels;
            set => _labels = value;
        }

        private bool _noData = false;
        private SeriesCollection _currencyGroups = new();
        private List<string> _labels = new();

        public Visibility NoDataVisible => _noData ? Visibility.Visible : Visibility.Hidden;

        public StatsViewModel()
        {
            log.Trace("Initializing StatsViewModel");

            GetTrades();
        }

        private void GetTrades()
        {
            var trades = AppService.Instance.GetCompletedTrades();

            if (trades.Count == 0)
            {
                _noData = true;
                OnPropertyChanged("NoDataVisible");
                return;
            }

            Offers = trades.Select(o => new StatsOffer()
                {
                    ItemName = o.ItemName,
                    PlayerName = o.PlayerName,
                    Price = o.Price.ToString(CultureInfo.InvariantCulture),
                    CurrencyImageLink = o.CurrencyImageLink ?? AppService.Instance.GetCurrencyImageLink(o.Currency)
                })
                .ToList();

            var groups = GroupByDate(trades.OrderBy(t => t.Time).ToList());
            var totalCurrency = 0;
            var currencies = new Dictionary<string, int>();

            foreach (var d in groups.Keys)
            {
                Labels.Add(d.ToString("dd MMM"));
                Trades[0].Values.Add(new ObservableValue(groups[d].Count));
                Currencies[0].Values.Add(new ObservableValue(groups[d].Sum(t =>
                    t.Currency.ToLower() == "chaos"
                        ? t.Price
                        : Math.Ceiling(AppService.Instance.GetChaosValueOfRealNameCurrency(t.Currency) * t.Price))));


                foreach (var currencyName in groups[d]
                    .Select(t => AppService.Instance.GetCurrencyRealName(t.Currency.ToLower())))
                {
                    if (!currencies.ContainsKey(currencyName))
                    {
                        currencies.Add(currencyName, 0);
                    }

                    ++currencies[currencyName];
                    ++totalCurrency;
                }
            }

            foreach (var c in currencies.Keys)
            {
                CurrencyGroups.Add(new PieSeries
                {
                    Title = c,
                    Values = new ChartValues<ObservableValue>()
                    {
                        new(Math.Round(currencies[c] / (double) totalCurrency, 2))
                    }
                });
            }
        }

        private static Dictionary<DateTime, List<Offer>> GroupByDate(IEnumerable<Offer> trades)
        {
            var grouped = new Dictionary<DateTime, List<Offer>>();

            foreach (var t in trades)
            {
                var d = DateTime.Parse($"{t.Time.Year}-{t.Time.Month}-{t.Time.Day}");

                if (!grouped.ContainsKey(d))
                {
                    grouped.Add(d, new List<Offer>());
                }

                grouped[d].Add(t);
            }

            return grouped;
        }
    }
}