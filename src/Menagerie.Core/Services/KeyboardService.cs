using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class KeyboardService : Service {
        #region Members
        private InputSimulator _input;
        #endregion

        #region COnstructors
        public KeyboardService() {
            _input = new InputSimulator();
        }
        #endregion

        #region Public methods
        public void KeyPress(VirtualKeyCode key) {
            _input.Keyboard.KeyPress(key);
        }

        public void KeyUp(VirtualKeyCode key) {
            _input.Keyboard.KeyUp(key);
        }

        public void KeyDown(VirtualKeyCode key) {
            _input.Keyboard.KeyDown(key);
        }

        public void ClearSpecialKeys() {
            KeyUp(VirtualKeyCode.CONTROL);
            KeyUp(VirtualKeyCode.SHIFT);
            KeyUp(VirtualKeyCode.MENU);
        }

        public void ModifiedKeyStroke(VirtualKeyCode modifier, VirtualKeyCode key) {
            _input.Keyboard.ModifiedKeyStroke(modifier, key);
        }

        public void SendEnter() {
            _input.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        public void SendCtrlA() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
        }

        public void SendBackspace() {
            _input.Keyboard.KeyPress(VirtualKeyCode.BACK);
        }

        public void SendCtrlV() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }
        #endregion
    }
}
