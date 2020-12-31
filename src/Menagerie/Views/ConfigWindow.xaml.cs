using log4net;
using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Menagerie.Core.Extensions;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window {
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
