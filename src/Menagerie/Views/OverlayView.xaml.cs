using log4net;
using Menagerie.Core.Services;
using Menagerie.Models;
using Menagerie.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Menagerie.Core.Extensions;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Menagerie.Services;
using Menagerie.Views;
using System.Threading;
using System.Threading.Tasks;
using AdonisUI.Controls;
using Point = System.Windows.Point;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class OverlayView : AdonisWindow
    {
        #region WinAPI

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(OverlayView));

        private readonly Rectangle _screenRect;
        private bool _winMoved;

        private Point _dragStart;
        private Vector _dragStartOffset;

        private OverlayViewModel _vm => (OverlayViewModel) DataContext;

        public OverlayView()
        {
            InitializeComponent();

            Log.Trace("Initializing Overlay");

            _screenRect = new Rectangle(0, 0, (int) SystemParameters.FullPrimaryScreenWidth,
                (int) SystemParameters.FullPrimaryScreenHeight);

            SourceInitialized += OverlayWindow_SourceInitialized;
            Loaded += OverlayWindow_Loaded;
            Activated += OverlayWindow_Activated;

            trayIcon.Icon = new Icon("Assets/menagerie-logo.ico");

            AppService.Instance.OnToggleOverlayVisibility += AppService_OnToggleOverlayVisibility;
            AppService.Instance.OnResetDefaultOverlay += AppService_OnResetDefaultOverlay;
            AppService.Instance.OnMouseMoved += AppService_OnOnMouseMoved;
        }

        private void AppService_OnOnMouseMoved()
        {
            if (_vm.MapModifiersPopupVisibility.Value == Visibility.Visible)
            {
                _vm.MapModifiersPopupVisibility.Value = Visibility.Hidden;
            }
        }

        private void AppService_OnResetDefaultOverlay()
        {
            SetOverlaysOffset();
        }

        private void OverlayWindow_Activated(object sender, EventArgs e)
        {
            OverlayViewModel.SetOverlayHandle(new WindowInteropHelper(this).Handle);
        }

        private void btnSendVirtualChatMessageInput_Click(object sender, RoutedEventArgs e)
        {
            var message = txtChatMessageInput.Text;

            if (string.IsNullOrEmpty(message) || message.Trim().Length == 0) return;

            OverlayViewModel.SendTranslatedMessage(message, (string) cboTargetLang.SelectedValue,
                (string) cboSourceLang.SelectedValue, true);
            HideTranslateInput();
        }

        private void btnTranslateInput_Click(object sender, RoutedEventArgs e)
        {
            var visible = stackChatMessageInput.Visibility == Visibility.Visible;

            if (visible)
            {
                AppService.Instance.FocusGame();
                HideTranslateInput();
            }
            else
            {
                Activate();
                ShowTranslateInput();
            }
        }

        private void ShowTranslateInput()
        {
            txtChatMessageInput.Text = "";

            _vm.ShowTranslateInputControl();

            Task.Delay(100)
                .ContinueWith(delegate { Application.Current.Dispatcher.Invoke(delegate { txtChatMessageInput.Focus(); }); });
        }

        private void HideTranslateInput()
        {
            txtChatMessageInput.Text = "";
            _vm.HideTranslateInputControl();
        }

        private void SetOverlaysOffset()
        {
            var config = _vm.Config;

            grdOffers_tt.X = config.IncomingOffersGridOffset.X;
            grdOffers_tt.Y = config.IncomingOffersGridOffset.Y;

            grdIncomingControls_tt.X = config.IncomingOffersControlsGridOffset.X;
            grdIncomingControls_tt.Y = config.IncomingOffersControlsGridOffset.Y;

            grdOutgoingControls_tt.X = config.OutgoingOffersGridOffset.X;
            grdOutgoingControls_tt.Y = config.OutgoingOffersGridOffset.Y;

            grdChaosRecipe_tt.X = config.ChaosRecipeGridOffset.X;
            grdChaosRecipe_tt.Y = config.ChaosRecipeGridOffset.Y;

            if (!config.ChaosRecipeEnabled) return;
            if (config.ChaosRecipeOveralyDockMode)
            {
                SetChaosRecipeOverlayDockMode();
            }
            else
            {
                SetChaosRecipeOverlayStackMode();
            }
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            SetOverlaysOffset();
        }

        private void OverlayWindow_SourceInitialized(object sender, EventArgs e)
        {
            NotificationService.Instance.Setup(trayIcon);
            OverlayViewModel.CheckUpdates();

            if (_winMoved) return;
            _winMoved = true;
            base.OnSourceInitialized(e);
            var wih = new WindowInteropHelper(this);
            var hWnd = wih.Handle;
            MoveWindow(hWnd, _screenRect.Left, _screenRect.Top, _screenRect.Width, _screenRect.Height, false);

            AppService.Instance.FocusGame();
        }

        private void AppService_OnToggleOverlayVisibility(bool show)
        {
            Log.Trace($"Toggling overlay visibility: {show}");

            Application.Current.Dispatcher.Invoke(delegate
            {
                if (show)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            });
        }

        private static void ShowStatsWindow()
        {
            Task.Run(() =>
            {
                while (!AppService.Instance.IsPoeNinjaCacheReady())
                {
                    Thread.Sleep(1000);
                }

                Application.Current.Dispatcher.Invoke(delegate { (new StatsView()).Show(); });
            });
        }

        private void btnBusy_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Busy button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();
            _vm.SendBusyWhisper((int) ((Button) sender).Tag);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Remove button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Button) sender).Tag;
            var offer = _vm.GetOffer(id);

            if (offer.PlayerInvited)
            {
                _vm.SendKick(id);
            }
            else
            {
                _vm.RemoveOffer(id);
            }
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Invite button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Button) sender).Tag;
            var offer = _vm.GetOffer(id);

            if (!offer.PlayerInvited)
            {
                _vm.SendInvite(id);
            }
            else
            {
                _vm.SendReInvite(id);
            }
        }

        private void grdOffer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Log.Trace("Offer clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Grid) sender).Tag;
            var offer = _vm.GetOffer(id);

            if (offer == null) return;
            var shiftKeyDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            var controlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (controlKeyDown && shiftKeyDown)
            {
                _vm.SendStillInterestedWhisper(id);
            }
            else if (shiftKeyDown)
            {
                _vm.HighlightItem(id);
            }
            else if (controlKeyDown)
            {
                _vm.SendSoldWhisper(id);
            }
            else
            {
                switch (offer.State)
                {
                    case OfferState.Initial:
                        _vm.SendInvite(id);
                        break;

                    case OfferState.PlayerInvited:
                    case OfferState.TradeRequestSent:
                        _vm.SendTradeRequest(id);
                        break;
                    case OfferState.Done:
                        break;
                    case OfferState.HideoutJoined:
                        break;
                }
            }
        }

        private void btnJoinHideout_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Join hideout button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Button) sender).Tag;

            _vm.SendJoinHideoutCommand(id);
        }

        private void btnTrade_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Trade button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Button) sender).Tag;

            _vm.SendTradeRequest(id, true);
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Leave button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            var id = (int) ((Button) sender).Tag;

            _vm.SendLeave(id);
        }

        private void btnClearOffers_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Clear offers button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            _vm.RemoveAllOffers();
        }

        private void btnClearOutgoingOffers_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Clear outgoing offers button clicked");
            AppService.Instance.FocusGame();
            AudioService.Instance.PlayClick();

            _vm.RemoveAllOffers(true);
        }

        private void txtSearchOutgoingOffer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.Trace($"Outgoing offers search bar input: {txtSearchOutgoingOffer.Text}");
            if (string.IsNullOrEmpty(txtSearchOutgoingOffer.Text))
            {
                _vm.ResetFilter(true);
            }
            else
            {
                _vm.FilterOffers(txtSearchOutgoingOffer.Text, true);
            }
        }

        private void txtSearchOffer_TextChanged(object sender, TextChangedEventArgs e)
        {
            Log.Trace($"Offers offers search bar input: {txtSearchOutgoingOffer.Text}");
            if (string.IsNullOrEmpty(txtSearchOffer.Text))
            {
                _vm.ResetFilter();
            }
            else
            {
                _vm.FilterOffers(txtSearchOffer.Text, false);
            }
        }

        private void itStats_Click(object sender, RoutedEventArgs e)
        {
            ShowStatsWindow();
        }

        private void itSettings_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Config system tray menu item clicked");
            _vm.ShowConfigWindow();
        }

        private void itQuit_Click(object sender, RoutedEventArgs e)
        {
            Log.Trace("Quit system tray menu item clicked");
            Application.Current.Shutdown(0);
        }

        private void grdChaosRecipe_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!_vm.OverlayMovable.Value)
            {
                grdChaosRecipe.Opacity = 0.1;
            }
        }

        private void grdChaosRecipe_MouseLeave(object sender, MouseEventArgs e)
        {
            grdChaosRecipe.Opacity = 0.7;
        }

        private void grdOffers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            _dragStart = e.GetPosition(this);
            _dragStartOffset = new Vector(grdOffers_tt.X, grdOffers_tt.Y);
            grdOffers.CaptureMouse();
        }

        private void grdOffers_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            if (!grdOffers.IsMouseCaptured) return;
            var offset = Point.Subtract(e.GetPosition(this), _dragStart);

            grdOffers_tt.X = _dragStartOffset.X + offset.X;
            grdOffers_tt.Y = _dragStartOffset.Y + offset.Y;
        }

        private void grdOffers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            grdOffers.ReleaseMouseCapture();
        }

        private void grdIncomingControls_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            _dragStart = e.GetPosition(this);
            _dragStartOffset = new Vector(grdIncomingControls_tt.X, grdIncomingControls_tt.Y);
            grdIncomingControls.CaptureMouse();
        }

        private void grdIncomingControls_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            grdIncomingControls.ReleaseMouseCapture();
        }

        private void grdIncomingControls_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            if (!grdIncomingControls.IsMouseCaptured) return;
            var offset = Point.Subtract(e.GetPosition(this), _dragStart);

            grdIncomingControls_tt.X = _dragStartOffset.X + offset.X;
            grdIncomingControls_tt.Y = _dragStartOffset.Y + offset.Y;
        }

        private void grdOutgoingControls_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            _dragStart = e.GetPosition(this);
            _dragStartOffset = new Vector(grdOutgoingControls_tt.X, grdOutgoingControls_tt.Y);
            grdOutgoingControls.CaptureMouse();
        }

        private void grdOutgoingControls_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            if (!grdOutgoingControls.IsMouseCaptured) return;
            var offset = Point.Subtract(e.GetPosition(this), _dragStart);

            grdOutgoingControls_tt.X = _dragStartOffset.X + offset.X;
            grdOutgoingControls_tt.Y = _dragStartOffset.Y + offset.Y;
        }

        private void grdOutgoingControls_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            grdOutgoingControls.ReleaseMouseCapture();
        }

        private void btnMoveOverlay_Click(object sender, RoutedEventArgs e)
        {
            _vm.ToggleMovableOverlay(grdOffers_tt, grdIncomingControls_tt, grdOutgoingControls_tt, grdChaosRecipe_tt,
                _vm.DockChaosRecipeOverlayVisibility.Value == Visibility.Visible);
        }

        private void grdChaosRecipe_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            _dragStart = e.GetPosition(this);
            _dragStartOffset = new Vector(grdChaosRecipe_tt.X, grdChaosRecipe_tt.Y);
            grdChaosRecipe.CaptureMouse();
        }

        private void grdChaosRecipe_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            if (!grdChaosRecipe.IsMouseCaptured) return;
            var offset = Point.Subtract(e.GetPosition(this), _dragStart);

            grdChaosRecipe_tt.X = _dragStartOffset.X + offset.X;
            grdChaosRecipe_tt.Y = _dragStartOffset.Y + offset.Y;
        }

        private void grdChaosRecipe_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_vm.OverlayMovable.Value) return;

            grdChaosRecipe.ReleaseMouseCapture();
        }

        private void SetChaosRecipeOverlayDockMode()
        {
            _vm.StackChaosRecipeOverlayVisibility.Value = Visibility.Hidden;
            _vm.DockChaosRecipeOverlayVisibility.Value = Visibility.Visible;
        }

        private void SetChaosRecipeOverlayStackMode()
        {
            _vm.DockChaosRecipeOverlayVisibility.Value = Visibility.Hidden;
            _vm.StackChaosRecipeOverlayVisibility.Value = Visibility.Visible;
        }

        private void ChangeChaosRecipeOverlayOrientation()
        {
            if (_vm.DockChaosRecipeOverlayVisibility.Value == Visibility.Visible)
            {
                SetChaosRecipeOverlayStackMode();
            }
            else
            {
                SetChaosRecipeOverlayDockMode();
            }
        }

        private void btnChangeChaosRecipeOrientation_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.OverlayMovable.Value)
            {
                ChangeChaosRecipeOverlayOrientation();
            }
        }

        private void BtnCloseTranslateInputControl_OnClick(object sender, RoutedEventArgs e)
        {
            HideTranslateInput();
        }

        private void TxtChatMessageInput_OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    HideTranslateInput();
                    break;
                case Key.Enter:
                    btnSendVirtualChatMessageInput_Click(sender, null);
                    break;
            }
        }
    }
}