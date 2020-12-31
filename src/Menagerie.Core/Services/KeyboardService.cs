using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput;
using WindowsInput.Native;
using Winook;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class KeyboardService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(KeyboardService));
        #endregion

        #region Members
        private InputSimulator _input = new InputSimulator();
        private KeyboardHook _keyboardHook;
        private MouseHook _mouseHook;
        #endregion

        #region Constructors
        public KeyboardService() {
            log.Trace("Initializing KeyboardService");
        }
        #endregion

        #region Public methods
        public void HookProcess(int processId) {
            log.Trace($"Hooking process {processId}");
            _keyboardHook = new KeyboardHook(processId);
            _keyboardHook.MessageReceived += KeyboardHook_MessageReceived;
            _keyboardHook.InstallAsync().Wait();
        }

        private void KeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e) {
            AppService.Instance.HandleKeyboardInput(e);
        }

        public void KeyPress(VirtualKeyCode key) {
            log.Trace($"Sending key press {(int)key}");
            _input.Keyboard.KeyPress(key);
        }

        public void KeyUp(VirtualKeyCode key) { 
              log.Trace($"Sending key up {(int)key}");
            _input.Keyboard.KeyUp(key);
        }

        public void KeyDown(VirtualKeyCode key) {
            log.Trace($"Sending key down {(int)key}");
            _input.Keyboard.KeyDown(key);
        }

        public void ClearSpecialKeys() {
            log.Trace($"Clearing special keys");
            KeyUp(VirtualKeyCode.CONTROL);
            KeyUp(VirtualKeyCode.SHIFT);
            KeyUp(VirtualKeyCode.MENU);
        }

        public void ModifiedKeyStroke(VirtualKeyCode modifier, VirtualKeyCode key) {
            log.Trace($"Sending modified key strokes for {(int)key} with {(int)modifier}");
            _input.Keyboard.ModifiedKeyStroke(modifier, key);
        }

        public void SendEnter() {
            log.Trace("Sending Enter key press");
            _input.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        public void SendCtrlA() {
            log.Trace("Sending Ctrl + A");
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
        }

        public void SendBackspace() {
            log.Trace("Sending Backspace key press");
            _input.Keyboard.KeyPress(VirtualKeyCode.BACK);
        }

        public void SendEscape() {
            log.Trace("Sending Escape key press");
            _input.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
        }

        public void SendCtrlV() {
            log.Trace("Sending Ctrl + V");
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        public void SendCtrlC() {
            log.Trace("Sending Ctrl + C");
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
        }

        public void Start() {
            log.Trace("Starting KeyboardService");
        }
        #endregion
    }
}
