using log4net;
using Menagerie.Core.Services;
using Menagerie.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using Menagerie.Core.Extensions;
using Menagerie.Services;

namespace Menagerie
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));

        private readonly SplashWindow _splash;
        private readonly OverlayWindow _overlay;

        public App()
        {
            InitializeComponent();

            Log.Trace("Initializing App", null);

            (new UpdateService()).CheckUpdates();

            _splash = new SplashWindow();
            _overlay = new OverlayWindow();
            _splash.Show();

            Task.Run(() =>
            {
                AppService.Instance.Start();

                Current.Dispatcher.Invoke(delegate { _splash.Close(); });
            });
        }

        private void Overlay_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Trace("Closing splash window");
            _splash.Close();
        }

        private void Application_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error("UI error: " + e.Exception.Message + Environment.NewLine + e.Exception.InnerException +
                      Environment.NewLine + e.Exception.StackTrace);

            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(0);
        }
    }
}