using Menagerie.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class GameService : IService {
        #region Constructors
        public GameService() {
        }
        #endregion

        #region Private methods
       
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

        public void Start() {
        }
        #endregion
    }
}
