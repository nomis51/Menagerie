using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core {
    public class GameHandler : Handler {
        #region Singleton
        private static GameHandler _instance;
        public static GameHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new GameHandler();
                }

                return _instance;
            }
        }
        #endregion

        public InputSimulator Input;

        public GameHandler() {
            Input = new InputSimulator();

            PoeWindowHandler.Instance.OnPoeWindowReady += PoeWindowHandler_OnPoeWindowReady;
        }

        private void PoeWindowHandler_OnPoeWindowReady() {
            
        }

        public override void Start() {
            base.Start();
        }

        private void ClearSpecialKeys() {
            Input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            Input.Keyboard.KeyUp(VirtualKeyCode.MENU);
        }

        public void HightlightStash(string searchText) {
            PoeWindowHandler.Instance.Focus();

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
