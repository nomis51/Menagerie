using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menagerie.Core.Services {
    public class ClipboardService : Service {
        #region Members
        private string LastText = "";
        #endregion

        #region Constructors
        public ClipboardService() {
            Listen();
        }
        #endregion

        #region Private methods
        private async void Listen() {
            while (true) {
                await Task.Delay(500);

                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text) && text != LastText) {
                    LastText = text;
                    AppService.Instance.NewClipboardText(text);
                }
            }
        }
        #endregion

        #region Public methods
        public bool SetClipboard(string value) {
            try {
                Clipboard.SetDataObject(value, true, 10, 100);
                return true;
            } catch (Exception e) {
                return false;
            }
            #endregion#
        }
    }
}
