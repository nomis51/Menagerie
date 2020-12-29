using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput;
using WindowsInput.Native;
using Winook;

namespace Menagerie.Core.Services {
    public class KeyboardService : IService {
        #region Members
        private InputSimulator _input = new InputSimulator();
        private KeyboardHook _keyboardHook;
        private MouseHook _mouseHook;
        #endregion

        #region Constructors
        public KeyboardService() {
        }
        #endregion

        #region Public methods
        public void HookProcess(int processId) {
            _keyboardHook = new KeyboardHook(processId);
            _keyboardHook.MessageReceived += KeyboardHook_MessageReceived;
            _keyboardHook.InstallAsync().Wait();
        }

        private void KeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e) {
            AppService.Instance.HandleKeyboardInput(e);
        }

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

        public void SendEscape() {
            _input.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        }

        public void SendCtrlV() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        public void SendCtrlC() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
        }

        public void Start() {
        }
        #endregion
    }
}
