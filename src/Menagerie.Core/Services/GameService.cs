using log4net;
using Menagerie.Core.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;
using System;
using System.Runtime.InteropServices;
using Desktop.Robot;
using Menagerie.Core.Win32;

namespace Menagerie.Core.Services
{
    public class GameService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(GameService));

        #endregion

        #region Members

        private bool _gameFocused;

        #endregion

        #region Constructors

        public GameService()
        {
            Log.Trace("Initializing GameService");
        }

        #endregion

        #region Private methods

        private static bool IsOverlayFocused()
        {
            var activeHandle = User32.GetForegroundWindow();
            var overlayHandle = AppService.Instance.GetOverlayHandle();
            return activeHandle == overlayHandle;
        }

        private void VerifyGameFocused()
        {
            Log.Trace("Verifying game focus");
            while (true)
            {
                var poeWinFocused = AppService.Instance.GameFocused();

                switch (poeWinFocused)
                {
                    case false when _gameFocused && !IsOverlayFocused():
                        Log.Trace("Game isn't focused");
                        _gameFocused = false;
                        AppService.Instance.HideOverlay();
                        AppService.Instance.EnsurePoeAlive();
                        break;
                    case true when !_gameFocused:
                        Log.Trace("Game is focused");
                        _gameFocused = true;
                        AppService.Instance.ShowOverlay();
                        break;
                }

                Thread.Sleep(500);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        #endregion

        #region Public methods

        public static void HighlightStash(string searchText)
        {
            Log.Trace($"Highlighting stash {searchText}");
            if (!AppService.Instance.FocusGame())
            {
                return;
            }

            AppService.Instance.ClearSpecialKeys();
            AppService.Instance.ModifiedKeyStroke(Key.Control, Key.F);

            try
            {
                AppService.Instance.SetClipboard(searchText);
                AppService.Instance.ClearSpecialKeys();
                AppService.Instance.ModifiedKeyStroke(Key.Control, Key.V);
            }
            catch (Exception e)
            {
                Log.Error("Error highlighting stash ", e);
            }
        }

        public void Start()
        {
            Log.Trace("Starting GameService");
            Task.Run(VerifyGameFocused);
        }

        #endregion
    }
}