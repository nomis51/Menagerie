using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class GameService : Service {
        #region Constructors
        public GameService() {
            AppService.Instance.OnPoeWindowReady += AppService_OnPoeWindowReady;
        }
        #endregion

        #region Private methods
        private void AppService_OnPoeWindowReady() {

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
            } catch {
                var g = 0;
            }

        }
        #endregion
    }
}
