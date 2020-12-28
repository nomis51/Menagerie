using Menagerie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Menagerie.ViewModels {
    public class OfferViewModel {
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

        private IList<Offer> _offers;

        public IList<Offer> Offers {
            get {
                return this._offers;
            }
            set {
                this._offers = value;
            }
        }

        public Offer Test { get; set; }

        public OfferViewModel() {
            Test = new Offer() {
                ItemName = "Saqawal"
            };
        }
    }
}
