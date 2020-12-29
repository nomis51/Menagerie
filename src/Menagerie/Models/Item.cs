using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Models {
    public class Item : INotifyPropertyChanged {
        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private string _name;
        public string Name {
            get {
                return _name;
            }
            set {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool _corrupted;
        public bool Corrupted {
            get {
                return _corrupted;
            }
            set {
                _corrupted = value;
                OnPropertyChanged("Corrupted");
            }
        }

        private string _icon;
        public string Icon {
            get {
                return _icon;
            }
            set {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        public Item(Core.Models.Item item) {
            Name = item.Name;
            Corrupted = item.IsCorrupted;
            Icon = item.Icon;
        }
    }
}
