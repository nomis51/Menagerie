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
using Menagerie.Core.Models;

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


        private PriceCheckResult _priceCheckResult;
        public PriceCheckResult PriceCheckResult {
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
                return PriceCheckResult.Item.IsCorrupted ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string AvgPriceText {
            get {
                return $"{PriceCheckResult.AvgPricing.Price}x";
            }
        }

        public string LowestPriceText {
            get {
                return $"{PriceCheckResult.LowestPricing.Price}x";
            }
        }

        public string HighestPriceText {
            get {
                return $"{PriceCheckResult.HighestPricing.Price}x";
            }
        }

        public string ModePriceText {
            get {
                return $"{PriceCheckResult.ModePricing.Price}x";
            }
        }

        public PriceCheckViewModel() {

        }

        public void SetPriceCheckResult(PriceCheckResult result) {
            PriceCheckResult = result;
        }
    }
}
