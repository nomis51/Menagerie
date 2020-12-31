using log4net;
using Menagerie.ViewModels;
using System.Windows;
using Menagerie.Core.Extensions;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigWindow));

        public ConfigViewModel vm;

        public ConfigWindow() {
            InitializeComponent();

            log.Trace("Initializing ConfigWindow");

            vm = new ConfigViewModel();
            DataContext = vm;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            log.Trace("Cancel button clicked");
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            log.Trace("Save button clicked");
            vm.SaveConfig();
            Close();
        }
    }
}
