using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class GameService : IService {
        #region Members
        private bool GameFocused = false;
        #endregion

        #region Constructors
        public GameService() { }
        #endregion

        #region Private methods
        private void VerifyGameFocused() {
            while (true) {
                bool poeWinFocused = AppService.Instance.GameFocused();

                if (!poeWinFocused && GameFocused) {
                    GameFocused = false;
                    AppService.Instance.HideOverlay();
                } else if(poeWinFocused && !GameFocused) {
                    GameFocused = true;
                    AppService.Instance.ShowOverlay();
                }

                Thread.Sleep(500);
            }
        }
        #endregion

        #region Public methods
        public void HightlightStash(string searchText) {
            AppService.Instance.FocusGame();

            AppService.Instance.ClearSpecialKeys();
            AppService.Instance.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_F);

            try {
                AppService.Instance.SetClipboard(searchText);
                AppService.Instance.ClearSpecialKeys();
                AppService.Instance.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
            } catch { }

        }

        public void Start() {
            Task.Run(() => VerifyGameFocused());
        }
        #endregion
    }
}
