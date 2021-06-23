using System;
using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using Menagerie.Core.Extensions;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace Menagerie.ViewModels
{
    public class SplashViewModel : Screen
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplashViewModel));

        public SplashViewModel()
        {
            Log.Trace("Initializing SplashViewModel");
        }
    }
}