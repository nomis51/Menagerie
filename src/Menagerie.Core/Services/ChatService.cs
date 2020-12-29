using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class ChatService : IService {
        #region Constructors
        public ChatService() {
        }
        #endregion

        #region Handlers
       
        #endregion

        #region Private methods
        private void Send(string message) {
            try {
                AppService.Instance.FocusGame();
                ClearSpecialKeys();
                AppService.Instance.SendEnter();
                AppService.Instance.SendCtrlA();
                AppService.Instance.SendBackspace();
                AppService.Instance.SetClipboard(message);
                AppService.Instance.SendCtrlV();
                AppService.Instance.SendEnter();
            } catch (Exception e) {
                var g = 0;
            }
        }

        private void ClearSpecialKeys() {
           AppService.Instance.KeyUp(VirtualKeyCode.CONTROL);
           AppService.Instance.KeyUp(VirtualKeyCode.SHIFT);
           AppService.Instance.KeyUp(VirtualKeyCode.MENU);
        }
        #endregion

        #region Public methods
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

        public void Start() {
        }
        #endregion
    }
}
