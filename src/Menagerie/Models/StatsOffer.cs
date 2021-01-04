﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Menagerie.Models {
   public class StatsOffer : INotifyPropertyChanged {
        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private string _itemName;
        public string ItemName {
            get {
                return _itemName;
            }
            set {
                _itemName = value;
                OnPropertyChanged("ItemName");
            }
        }

        private string _playerName;
        public string PlayerName {
            get {
                return _playerName;
            }
            set {
                _playerName = value;
                OnPropertyChanged("PlayerName");
            }
        }

        private string _price;
        public string Price {
            get {
                return _price;
            }
            set {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        private string _currentImageLink;
        public string CurrencyImageLink {
            get {
                return _currentImageLink;
            }
            set {
                _currentImageLink = value;
                OnPropertyChanged("CurrencyImageLink");
            }
        }

        public StatsOffer() { }


    }
}
