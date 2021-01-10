using log4net;
using Menagerie.ViewModels;
using System.Windows;
using Menagerie.Core.Extensions;
using AdonisUI.Controls;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : AdonisWindow {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigWindow));

        public ConfigViewModel vm;

        private List<Grid> views = new List<Grid>();

        public ConfigWindow() {
            InitializeComponent();

            log.Trace("Initializing ConfigWindow");

            vm = new ConfigViewModel();
            DataContext = vm;

            SetupViews();
            ShowView("grdGeneral");
        }

        private void SetupViews() {
            views.Add(grdApi);
            views.Add(grdGeneral);
            views.Add(grdWhispers);
            views.Add(grdChatScan);
            views.Add(grdChaosRecipe);
        }
        private void ShowView(string name) {
            foreach (var v in views) {
                v.Visibility = v.Name == name ? Visibility.Visible : Visibility.Hidden;
            }
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

        private void itGeneral_Click(object sender, RoutedEventArgs e) {
            ShowView("grdGeneral");
        }

        private void itApi_Click(object sender, RoutedEventArgs e) {
            ShowView("grdApi");
        }

        private void itWhispers_Click(object sender, RoutedEventArgs e) {
            ShowView("grdWhispers");
        }

        private void itChatScan_Click(object sender, RoutedEventArgs e) {
            ShowView("grdChatScan");
        }

        private void itChaosRecipe_Click(object sender, RoutedEventArgs e) {
            ShowView("grdChaosRecipe");
        }
    }
}
