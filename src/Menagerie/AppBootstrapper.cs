using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Menagerie.ViewModels;

namespace Menagerie
{
    public class AppBootstrapper : BootstrapperBase
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AppBootstrapper));
        
        public AppBootstrapper()
        {   
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<OverlayViewModel>();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
           Log.Error("Unhandled Exception", e.Exception);
        }
    }
}