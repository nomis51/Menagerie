using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core {
    public class ChatHandler : Handler {
        #region WinAPI
        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
        #endregion

        #region Singleton
        private static ChatHandler _instance;
        public static ChatHandler Instance {
            get {
                if (_instance == null) {
                    _instance = new ChatHandler();
                }

                return _instance;
            }
        }
        #endregion

        private InputSimulator Input;

        private ChatHandler() {
            Input = new InputSimulator();
            PoeWindowHandler.Instance.OnPoeWindowReady += PoeWindowHandler_OnPoeWindowReady;
        }

        private void PoeWindowHandler_OnPoeWindowReady() {
            Ready = true;
        }

        public override void Start() {
            base.Start();
        }

        private void SendEnter() {
            Input.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        private void SendKeyWithControl(VirtualKeyCode key) {
            Input.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
            Input.Keyboard.KeyPress(key);
            Input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
        }

        private void SendCtrlA() {
            SendKeyWithControl(VirtualKeyCode.VK_A);
        }

        private void SendBackspace() {
            Input.Keyboard.KeyPress(VirtualKeyCode.BACK);
        }

        private void SendCtrlV() {
            SendKeyWithControl(VirtualKeyCode.VK_V);
        }

        private void ToClipboard(string text) {
            try {
                Clipboard.SetDataObject(text, true, 10, 100);
            } catch (Exception e) {
                var g = 0;
            }
        }


        private void Send(string message) {
            try {
                PoeWindowHandler.Instance.Focus();
                ClearSpecialKeys();
                SendEnter();
                SendCtrlA();
                SendBackspace();
                ToClipboard(message);
                SendCtrlV();
                SendEnter();
            } catch (Exception e) {
                var g = 0;
            }
        }

        private void ClearSpecialKeys() {
            Input.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            Input.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            Input.Keyboard.KeyUp(VirtualKeyCode.MENU);
        }

        public void SendChatMessage(string message) {
            Send(message);
        }

        public void SendHideoutCommand(string playerName) {
            Send($"/hideout {playerName}");
        }

        public void SendInviteCommand(string playerName) {
            Send($"/invite {playerName}");
        }

        public void SendKickCommand(string playerName) {
            Send($"/kick {playerName}");
        }

        public void SendTradeCommand(string playerName) {
            Send($"/tradewith {playerName}");
        }
    }
}
