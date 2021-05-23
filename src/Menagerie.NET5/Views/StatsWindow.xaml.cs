using log4net;
using Menagerie.ViewModels;
using Menagerie.Core.Extensions;
using AdonisUI.Controls;
using System.Windows.Media;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour StatsWindow.xaml
    /// </summary>
    public partial class StatsWindow : AdonisWindow
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(StatsWindow));

        public StatsWindow()
        {
            InitializeComponent();

            Log.Trace("Initializing StatsWindow");

            DataContext = new StatsViewModel();

            SetupCharts();
        }

        private void SetupCharts()
        {
            Log.Trace("Setup charts");
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