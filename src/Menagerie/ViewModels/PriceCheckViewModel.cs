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
using Item = Menagerie.Core.Models.Item;

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

        private string _poeNinjaChaosValue;
        public string PoeNinjaChaosValueText {
            get {
                return _poeNinjaChaosValue == (0.0d).ToString() ? "N/A" : $"{_poeNinjaChaosValue}x";
            }
            set {
                _poeNinjaChaosValue = value;
                OnPropertyChanged("PoeNinjaChaosValueText");
            }
        }

        private string _chaosImageLink;
        public string ChaosImageLink {
            get {
                return _chaosImageLink;
            }
            set {
                _chaosImageLink = value;
                OnPropertyChanged("ChaosImageLink");
            }
        }

        private PriceCheckResult _priceCheckResult;
        public PriceCheckResult PriceCheckResult {
            get {
                return _priceCheckResult;
            }
            set {
                _priceCheckResult = value;
                OnPropertyChanged("PriceCheckResult");
                OnPropertyChanged("AvgPriceText");
                OnPropertyChanged("LowestPriceText");
                OnPropertyChanged("HighestPriceText");
                OnPropertyChanged("ModePriceText");
                OnPropertyChanged("ItemIcon");
            }
        }

        public string ItemIcon {
            get {
                return PriceCheckResult == null ? Item.Icon : PriceCheckResult.Item.Icon;
            }
        }

        public Visibility CorruptedVisible {
            get {
                return Item.IsCorrupted ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string AvgPriceText {
            get {
                if (PriceCheckResult == null) {
                    return "...";
                }

                return $"{PriceCheckResult.AvgPricing.Price}x";
            }
        }

        public string LowestPriceText {
            get {
                if (PriceCheckResult == null) {
                    return "...";
                }

                return $"{PriceCheckResult.LowestPricing.Price}x";
            }
        }

        public string HighestPriceText {
            get {
                if (PriceCheckResult == null) {
                    return "...";
                }

                return $"{PriceCheckResult.HighestPricing.Price}x";
            }
        }

        public string ModePriceText {
            get {
                if (PriceCheckResult == null) {
                    return "...";
                }

                return $"{PriceCheckResult.ModePricing.Price}x";
            }
        }

        public PriceCheckViewModel() {

        }

        public void SetPriceCheckResult(PriceCheckResult result) {
            PriceCheckResult = result;
        }

        public void SetItem(Item item, double poeNinjaChaosValue, string chaosImageLink) {
            Item = item;
            PoeNinjaChaosValueText = poeNinjaChaosValue.ToString();
            ChaosImageLink = chaosImageLink;
        }
    }
}
