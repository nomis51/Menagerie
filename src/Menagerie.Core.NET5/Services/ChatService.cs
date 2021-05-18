using log4net;
using Menagerie.Core.Abstractions;
using System;
using WindowsInput.Native;
using Menagerie.Core.Extensions;
using System.Threading;

namespace Menagerie.Core.Services {
    public class ChatService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(ChatService));
        #endregion

        #region Constructors
        public ChatService() {
            log.Trace("Initializing ChatService");
        }
        #endregion

        #region Handlers
       
        #endregion

        #region Private methods
        private void Send(string message, int delay = 0) {
            log.Trace("Sending message");
            try {
                if (!AppService.Instance.FocusGame()) {
                    return;
                }

                if(delay > 0) {
                    Thread.Sleep(delay);
                }

                ClearSpecialKeys();
                AppService.Instance.SendEnter();
                AppService.Instance.SendCtrlA();
                AppService.Instance.SendBackspace();
                AppService.Instance.SetClipboard(message);
                AppService.Instance.SendCtrlV();
                AppService.Instance.SendEnter();
            } catch (Exception e) {
                log.Error("Error while sending message", e);
            }
        }

        private void ClearSpecialKeys() {
            log.Trace("Clearing special keys");
           AppService.Instance.KeyUp(VirtualKeyCode.CONTROL);
           AppService.Instance.KeyUp(VirtualKeyCode.SHIFT);
           AppService.Instance.KeyUp(VirtualKeyCode.MENU);
        }
        #endregion

        #region Public methods
        public void SendChatMessage(string message, int delay = 0) {
            log.Trace("Seding chat message");
            Send(message, delay);
        }

        public void SendHideoutCommand() {
            log.Trace("Sending hideout command");
            Send("/hideout");
        }

        public void SendHideoutCommand(string playerName) {
            log.Trace("Sending hideout command with param");
            Send($"/hideout {playerName}");
        }

        public void SendInviteCommand(string playerName) {
            log.Trace("Sending invite command");
            Send($"/invite {playerName}");
        }

        public void SendKickCommand(string playerName) {
            log.Trace("Sending kick command");
            Send($"/kick {playerName}");
        }

        public void SendTradeCommand(string playerName) {
            log.Trace("Sending trace command");
            Send($"/tradewith {playerName}");
        }

        public void Start() {
            log.Trace("Starting ChatService");
        }
        #endregion
    }
}
