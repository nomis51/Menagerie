using Menagerie.ViewModels;
using AdonisUI.Controls;
using System.Windows.Media;
using Serilog;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour StatsWindow.xaml
    /// </summary>
    public partial class StatsView : AdonisWindow
    {
        public StatsView()
        {
            InitializeComponent();

            Log.Information("Initializing StatsWindow");

            DataContext = new StatsViewModel();

            SetupCharts();
        }

        private void SetupCharts()
        {
            Log.Information("Setup charts");
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