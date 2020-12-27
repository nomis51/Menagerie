using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toucan.DTOs;

namespace Toucan.Models {
    public class Config : INotifyPropertyChanged {
        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private int _id;
        public int Id {
            get {
                return _id;
            }
            set {
                _id = value;
                OnPropertyChanged("Id");
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

        private string _currentLeague;
        public string CurrentLeague {
            get {
                return _currentLeague;
            }
            set {
                _currentLeague = value;
                OnPropertyChanged("CurrentLeague");
            }
        }

        private bool _onlyShowOffersOfCurrentLeague;
        public bool OnlyShowOffersOfCurrentLeague {
            get {
                return _onlyShowOffersOfCurrentLeague;
            }
            set {
                _onlyShowOffersOfCurrentLeague = value;
                OnPropertyChanged("OnlyShowOffersOfCurrentLeague");
            }
        }

        public Config() { }

        public Config(ConfigDto dto) {
            Id = dto.Id;
            PlayerName = dto.PlayerName;
            CurrentLeague = dto.CurrentLeague;
            OnlyShowOffersOfCurrentLeague = dto.OnlyShowOffersOfCurrentLeague;
        }
    }
}
