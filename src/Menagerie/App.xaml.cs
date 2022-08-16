using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;
using Menagerie.Application.Services;
using ReactiveUI;
using Serilog;
using Splat;

namespace Menagerie
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MessageBox.Show("Menagerie only supports Microsoft Windows operating system. The app will now exit.",
                    "OS mismatch", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }

            AppService.Instance.Initialize();
            AppService.Instance.Start().Wait();

            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled application exception: {message}. {stackTrace", e.Exception.Message,
                e.Exception.StackTrace);
            Environment.Exit(-1);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            AppService.Instance.OnAppExit();
        }
    }
}