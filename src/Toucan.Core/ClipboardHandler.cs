using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toucan.Core {
    public class ClipboardHandler : Handler {
        #region Events
        public delegate void NewClipboardText(string text);
        public event NewClipboardText OnNewClipboardText;
        #endregion

        #region Singleton
        private static ClipboardHandler _instance;
        public static ClipboardHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new ClipboardHandler();
                }

                return _instance;
            }
        }
        #endregion

        private string LastText = "";

        private ClipboardHandler() { }

        public override void Start() {
            base.Start();

            Listen();
        }

        private async void Listen() {
            while (true) {
                await Task.Delay(500);

                string text = Clipboard.GetText();

                if (!string.IsNullOrEmpty(text) && text != LastText) {
                    LastText = text;
                    OnNewClipboardText(text);
                }
            }
        }
    }
}
