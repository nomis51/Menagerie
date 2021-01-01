using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Menagerie.ViewModels {
    public class StatsViewModel : INotifyPropertyChanged {
        #region Updater
        private ICommand mUpdater;
        public ICommand UpdateCommand {
            get {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set {
                mUpdater = value;
            }
        }

        private class Updater : ICommand {
            #region ICommand Members  

            public bool CanExecute(object parameter) {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter) {

            }

            #endregion
        }
        #endregion

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private readonly static ILog log = LogManager.GetLogger(typeof(StatsViewModel));

        public SeriesCollection Trades { get; set; } = new SeriesCollection() {
                new LineSeries() {
                    AreaLimit = 0,
                    Values = new ChartValues<ObservableValue>()
                }
            };
        public SeriesCollection Currencies { get; set; } = new SeriesCollection() {
                new LineSeries() {
                    AreaLimit = 0,
                    Values = new ChartValues<ObservableValue>()
                }
            };

        public List<string> Labels { get; set; } = new List<string>();

        public StatsViewModel() {
            log.Trace("Initializing StatsViewModel");

            GetTrades();
        }

        private void GetTrades() {
            // var trades = AppService.Instance.GetCompletedTrades();

            List<Offer> trades = new List<Offer>() {
                new Offer() {
                    Time = DateTime.Now.AddDays(-5),
                    Price = 50
                },
                  new Offer() {
                    Time = DateTime.Now.AddDays(-5),
                    Price = 1
                },
                new Offer() {
                    Time = DateTime.Now.AddDays(-3),
                    Price  =120
                },
                new Offer() {
                    Time = DateTime.Now.AddDays(-3),
                    Price = 5
                },
                 new Offer() {
                    Time = DateTime.Now.AddDays(-3),
                     Price = 23
                },
                  new Offer() {
                    Time = DateTime.Now,
                    Price =132
                }
            };

            var groups = GroupByDate(trades);

            foreach (var d in groups.Keys) {
                Labels.Add(d.ToString("dd MMM"));
                Trades[0].Values.Add(new ObservableValue(groups[d].Count()));
                Currencies[0].Values.Add(new ObservableValue(groups[d].Sum(t => t.Currency.ToLower() == "chaos" ? t.Price : (AppService.Instance.GetChaosValueOfCurrency(t.Currency) * t.Price))));
            }
        }

        private Dictionary<DateTime, List<Offer>> GroupByDate(List<Offer> trades) {
            Dictionary<DateTime, List<Offer>> grouped = new Dictionary<DateTime, List<Offer>>();

            foreach (var t in trades) {
                var d = DateTime.Parse($"{t.Time.Year}-{t.Time.Month}-{t.Time.Day}");

                if (!grouped.ContainsKey(d)) {
                    grouped.Add(d, new List<Offer>());
                }

                grouped[d].Add(t);
            }

            return grouped;
        }
    }
}
