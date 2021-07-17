using Caliburn.Micro;
using Serilog;

namespace Menagerie.ViewModels
{
    public class SplashViewModel : Screen
    {
        public SplashViewModel()
        {
            Log.Information("Initializing SplashViewModel");
        }
    }
}