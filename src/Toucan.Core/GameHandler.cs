using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Toucan.Core {
    public class GameHandler {

        private PoeWindow Poe;
        public InputSimulator Input;

        public GameHandler(PoeWindow poe) {
            Input = new InputSimulator();
            Poe = poe;
        }
        private void ClearSpecialKeys() {
            Input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            Input.Keyboard.KeyUp(VirtualKeyCode.MENU);
        }

        public void HightlightStash(string searchText) {
            Poe.Focus();

            ClearSpecialKeys();
            Input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_F);

            try {
                Clipboard.SetDataObject(searchText, true, 10, 100);
                ClearSpecialKeys();
                Input.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            } catch {
                var g = 0;
            }

        }
    }
}
