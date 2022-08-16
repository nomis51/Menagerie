using Menagerie.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows;
using Serilog;
using Serilog.Core;

namespace Menagerie
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.File("Menagerie-log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Initializing App");

            Task.Run(() => { AppService.Instance.Start(); });
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