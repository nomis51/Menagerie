using log4net;
using Menagerie.Core.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;
using Menagerie.Core.Extensions;
using System;
using System.Runtime.InteropServices;

namespace Menagerie.Core.Services {
    public class GameService : IService {
        #region WinAPI
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();
        #endregion

        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(GameService));
        #endregion

        #region Members
        private bool GameFocused = false;
        #endregion

        #region Constructors
        public GameService() {
            log.Trace("Initializing GameService");
        }
        #endregion

        #region Private methods
        private bool IsOverlayFocused() {
            IntPtr activeHandle = GetForegroundWindow();
            IntPtr overlayHandle = AppService.Instance.GetOverlayHandle();
            return activeHandle == overlayHandle;
        }

        private void VerifyGameFocused() {
            log.Trace("Verifying game focus");
            while (true) {
                bool poeWinFocused = AppService.Instance.GameFocused();

                if (!poeWinFocused && GameFocused && !IsOverlayFocused()) {
                    log.Trace("Game isn't focused");
                    GameFocused = false;
                    AppService.Instance.HideOverlay();
                    AppService.Instance.EnsurePoeAlive();
                } else if (poeWinFocused && !GameFocused) {
                    log.Trace("Game is focused");
                    GameFocused = true;
                    AppService.Instance.ShowOverlay();
                }

                Thread.Sleep(500);
            }
        }
        #endregion

        #region Public methods
        public void HightlightStash(string searchText) {
            log.Trace($"Highlighting stash {searchText}");
            if (!AppService.Instance.FocusGame()) {
                return;
            }

            AppService.Instance.ClearSpecialKeys();
            AppService.Instance.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_F);

            try {
                AppService.Instance.SetClipboard(searchText);
                AppService.Instance.ClearSpecialKeys();
                AppService.Instance.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            } catch (Exception e) {
                log.Error("Error highlighting stash ", e);
            }

        }

        public void Start() {
            log.Trace("Starting GameService");
            Task.Run(() => VerifyGameFocused());
        }
        #endregion
    }
}
