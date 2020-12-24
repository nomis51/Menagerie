using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toucan.Core {
    public class ClipboardListener {
        public delegate void NewClipboardText(string text);
        public event NewClipboardText OnNewClipboardText;

        private string LastText = "";

        public ClipboardListener() {
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
