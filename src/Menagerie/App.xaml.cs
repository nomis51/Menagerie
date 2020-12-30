using Menagerie.Core.Services;
using Menagerie.ViewModels;
using Menagerie.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Menagerie {
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application {
        private OverlayWindow overlay;
        private SplashWindow splash;

        public App() {
            InitializeComponent();
            splash = new SplashWindow();
            overlay = new OverlayWindow();
            splash.Show();

            Task.Run(() => {
                AppService.Instance.Start();

                App.Current.Dispatcher.Invoke(delegate {
                    splash.Close();
                    overlay.Show();
                });
            });
        }

       

        private void Overlay_Loaded(object sender, RoutedEventArgs e) {
            splash.Close();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            File.WriteAllText(".\\ui-errors.log", e.Exception.Message + Environment.NewLine + e.Exception.InnerException + Environment.NewLine + e.Exception.StackTrace);

            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(0);
        }
    }
}
