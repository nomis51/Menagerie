using log4net;
using Menagerie.Core.Services;
using Menagerie.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using Menagerie.Core.Extensions;
using Forms = System.Windows.Forms;
using System.Threading;

namespace Menagerie {
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        private OverlayWindow overlay;
        private SplashWindow splash;

        public App() {
            InitializeComponent();

            log.Trace("Initializing App", null);

            splash = new SplashWindow();
            overlay = new OverlayWindow(Forms.Screen.PrimaryScreen);
            splash.Show();

            Task.Run(() => {
                AppService.Instance.Start();

                App.Current.Dispatcher.Invoke(delegate {
                    splash.Close();
                });
            });
        }

        private void Overlay_Loaded(object sender, RoutedEventArgs e) {
            log.Trace("Closing splash window");
            splash.Close();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            log.Error("UI error: " + e.Exception.Message + Environment.NewLine + e.Exception.InnerException + Environment.NewLine + e.Exception.StackTrace);

            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(0);
        }
    }
}
