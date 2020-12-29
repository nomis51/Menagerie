using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CoreModels = Menagerie.Core.Models;
using Menagerie.Models;

namespace Menagerie.ViewModels {
    public class PriceCheckViewModel : INotifyPropertyChanged {
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

        private Item _item;
        public Item Item {
            get {
                return _item;
            }
            set {
                _item = value;
                OnPropertyChanged("Item");
            }
        }

        private CoreModels.PriceCheckResult _priceCheckResult;
        public CoreModels.PriceCheckResult PriceCheckResult {
            get {
                return _priceCheckResult;
            }
            set {
                _priceCheckResult = value;
                OnPropertyChanged("PriceCheckResult");
            }
        }

        public Visibility CorruptedVisible {
            get {
                return Item.Corrupted ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public PriceCheckViewModel() {

        }
    }
}
