using Menagerie.Core.Abstractions;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Runtime.InteropServices;
using Desktop.Robot;
using Serilog;

namespace Menagerie.Core.Services
{
    public class GameService : IService
    {
        #region WinAPI

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        #endregion

        #region Members

        private bool _gameFocused;

        #endregion

        #region Constructors

        public GameService()
        {
            Log.Information("Initializing GameService");
        }

        #endregion

        #region Private methods

        private static bool IsOverlayFocused()
        {
            var activeHandle = GetForegroundWindow();
            var overlayHandle = AppService.Instance.GetOverlayHandle();
            return activeHandle == overlayHandle;
        }

        private void VerifyGameFocused()
        {
            Log.Information("Verifying game focus");
            while (true)
            {
                var poeWinFocused = AppService.Instance.GameFocused();

                switch (poeWinFocused)
                {
                    case false when _gameFocused && !IsOverlayFocused():
                        Log.Information("Game isn't focused");
                        _gameFocused = false;
                        AppService.Instance.HideOverlay();
                        AppService.Instance.EnsurePoeAlive();
                        break;
                    case true when !_gameFocused:
                        Log.Information("Game is focused");
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
            Log.Information($"Highlighting stash {searchText}");
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
            Log.Information("Starting GameService");
            Task.Run(VerifyGameFocused);
        }

        #endregion
    }
}