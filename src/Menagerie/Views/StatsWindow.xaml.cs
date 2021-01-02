using log4net;
using Menagerie.ViewModels;
using System.Windows;
using Menagerie.Core.Extensions;
using AdonisUI.Controls;
using System.Windows.Media;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : AdonisWindow {
        public StatsViewModel vm;

        private readonly static ILog log = LogManager.GetLogger(typeof(StatsWindow));

        public StatsWindow() {
            InitializeComponent();

            log.Trace("Initializing StatsWindow");

            vm = new StatsViewModel();
            DataContext = vm;

            SetupCharts();
        }

        private void SetupCharts() {
            log.Trace("Setup charts");
            chCurrencies.DataTooltip.Background = Brushes.Black;
            chCurrencies.AxisY[0].MinValue = 0;
            chCurrencies.AxisX[0].MinValue = 0;
            chTrades.DataTooltip.Background = Brushes.Black;
            chTrades.AxisY[0].MinValue = 0;
            chCurrencies.AxisX[0].MinValue = 0;
            chCurrencyGroups.DataTooltip.Background = Brushes.Black;
        }
    }
}
