using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Menagerie.ViewModels;
using Serilog;

namespace Menagerie
{
    public class AppBootstrapper : BootstrapperBase
    {
        public static readonly IWindowManager WindowManager = new WindowManager();

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<OverlayViewModel>().Wait();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error("Unhandled Exception", e.Exception);
        }
    }
}