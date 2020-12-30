using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Menagerie.Core.Services {
    public class ClipboardService : IService {
        #region Members
        private string LastText = "";
        #endregion

        #region Constructors
        public ClipboardService() {
        }
        #endregion

        #region Private methods
        private async void Listen() {
            while (true) {
                await Task.Delay(500);

                string text = GetClipboard();

                if (!string.IsNullOrEmpty(text) && text != LastText) {
                    LastText = text;
                    AppService.Instance.NewClipboardText(text);
                }
            }
        }
        #endregion

        #region Public methods
        public bool SetClipboard(string value) {
            bool result = false;

            Thread t = new Thread(() => {
                try {
                    Clipboard.SetDataObject(value, true, 10, 100);
                    result = true;
                } catch (Exception e) { }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return result;
        }
        #endregion

        public string GetClipboard() {
            string text = "";

            Thread t = new Thread(delegate () {
                text = Clipboard.GetText();
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return text;
        }

        public void Start() {
            Listen();
        }
    }
}
