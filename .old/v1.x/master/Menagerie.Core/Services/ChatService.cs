using Menagerie.Core.Abstractions;
using System;
using System.Threading;
using Desktop.Robot;
using Serilog;

namespace Menagerie.Core.Services
{
    public class ChatService : IService
    {
        #region Constructors

        public ChatService()
        {
            Log.Information("Initializing ChatService");
        }

        #endregion

        #region Handlers

        #endregion

        #region Private methods

        private static void Send(string message, int delay = 0)
        {
            Log.Information("Sending message");
            try
            {
                if (!AppService.Instance.FocusGame())
                {
                    return;
                }

                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }

                ClearSpecialKeys();
                AppService.Instance.SendEnter();
                AppService.Instance.SendCtrlA();
                AppService.Instance.SendBackspace();
                AppService.Instance.SetClipboard(message);
                AppService.Instance.SendCtrlV();
                AppService.Instance.SendEnter();
            }
            catch (Exception e)
            {
                Log.Error("Error while sending message", e);
            }
        }

        private static void ClearSpecialKeys()
        {
            Log.Information("Clearing special keys");
            AppService.Instance.KeyUp(Key.Control);
            AppService.Instance.KeyUp(Key.Shift);
        }

        #endregion

        #region Public methods

        public static void SendChatMessage(string message, int delay = 0)
        {
            Log.Information("Sending chat message");
            Send(message, delay);
        }

        public static void SendHideoutCommand()
        {
            Log.Information("Sending hideout command");
            Send("/hideout");
        }

        public static void SendHideoutCommand(string playerName)
        {
            Log.Information("Sending hideout command with param");
            Send($"/hideout {playerName}");
        }

        public static void SendInviteCommand(string playerName)
        {
            Log.Information("Sending invite command");
            Send($"/invite {playerName}");
        }

        public static void SendKickCommand(string playerName)
        {
            Log.Information("Sending kick command");
            Send($"/kick {playerName}");
        }

        public static void SendTradeCommand(string playerName)
        {
            Log.Information("Sending Information command");
            Send($"/tradewith {playerName}");
        }

        public void Start()
        {
            Log.Information("Starting ChatService");
        }

        #endregion
    }
}