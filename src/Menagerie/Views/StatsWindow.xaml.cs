using log4net;
using Menagerie.ViewModels;
using System.Windows;
using Menagerie.Core.Extensions;
using AdonisUI.Controls;

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
        }
    }
}
