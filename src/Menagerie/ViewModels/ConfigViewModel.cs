using Menagerie.Core.Models;
using Menagerie.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Menagerie.ViewModels {
    public class ConfigViewModel : INotifyPropertyChanged {
        #region Updater
        private ICommand mUpdater;
        public ICommand UpdateCommand {
            get {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set {
                mUpdater = value;
            }
        }

        private class Updater : ICommand {
            #region ICommand Members  

            public bool CanExecute(object parameter) {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter) {

            }

            #endregion
        }
        #endregion

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private Config _config;
        public Config Config {
            get {
                return _config;
            }
            set {
                _config = value;
                OnPropertyChanged("Config");
            }
        }

        private ObservableCollection<string> _leagues = new ObservableCollection<string>();
        public ObservableCollection<string> Leagues {
            get {
                return _leagues;
            }
            set {
                _leagues = value;
                OnPropertyChanged("Leagues");
            }
        }

        public ConfigViewModel() {
            GetLeagues();
            GetConfig();
        }

        public void SaveConfig() {
            AppService.Instance.SetConfig(Config);
        }

        private void GetConfig() {
            Config = AppService.Instance.GetConfig();
        }

        private void GetLeagues() {
            var leagues = AppService.Instance.GetLeagues().Result;

            foreach (var league in leagues) {
                Leagues.Add(league);
            }
        }
    }
}
