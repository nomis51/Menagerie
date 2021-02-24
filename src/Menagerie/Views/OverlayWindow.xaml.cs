using log4net;
using Menagerie.Core.Services;
using Menagerie.Models;
using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Forms = System.Windows.Forms;
using Menagerie.Core.Extensions;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Menagerie.Services;
using Menagerie.Views;
using System.Threading;
using System.Threading.Tasks;
using AdonisUI.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using System.Text.RegularExpressions;
using Menagerie.Controls;

namespace Menagerie {
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class OverlayWindow : AdonisWindow {
        #region WinAPI
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        #endregion

        private static readonly ILog log = LogManager.GetLogger(typeof(OverlayWindow));

        private readonly System.Drawing.Rectangle screenRect;
        private bool WinMoved = false;

        private Point DragStart;
        private Vector DragStartOffet;



        public OverlayViewModel vm;

        public OverlayWindow(Forms.Screen screen) {
            InitializeComponent();

            log.Trace("Initializing Overlay");

            screenRect = screen.WorkingArea;

            vm = new OverlayViewModel();
            this.DataContext = vm;


            this.SourceInitialized += OverlayWindow_SourceInitialized;
            this.Loaded += OverlayWindow_Loaded;
            this.Activated += OverlayWindow_Activated;

            AppService.Instance.OnToggleOverlayVisibility += AppService_OnToggleOverlayVisibility;
        }

        private void OverlayWindow_Activated(object sender, EventArgs e) {
            vm.SetOverlayHandle(new WindowInteropHelper(this).Handle);
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;

            var config = vm.Config;

            grdOffers_tt.X = config.IncomingOffersGridOffset.X;
            grdOffers_tt.Y = config.IncomingOffersGridOffset.Y;

            grdIncomingControls_tt.X = config.IncomingOffersControlsGridOffset.X;
            grdIncomingControls_tt.Y = config.IncomingOffersControlsGridOffset.Y;

            grdOutgoingControls_tt.X = config.OutgoingOffersGridOffset.X;
            grdOutgoingControls_tt.Y = config.OutgoingOffersGridOffset.Y;
        }

        private void OverlayWindow_SourceInitialized(object sender, EventArgs e) {
            NotificationService.Instance.Setup(trayIcon);

            if (!WinMoved) {
                WinMoved = true;
                base.OnSourceInitialized(e);
                var wih = new WindowInteropHelper(this);
                IntPtr hWnd = wih.Handle;
                MoveWindow(hWnd, screenRect.Left, screenRect.Top, screenRect.Width, screenRect.Height, false);
            }
        }

        private void AppService_OnToggleOverlayVisibility(bool show) {
            log.Trace($"Toggling overlay visibility: {show}");

            App.Current.Dispatcher.Invoke(delegate {
                if (show) {
                    Hide();
                } else {
                    Show();
                }
            });
        }

        private void ShowStatsWindow() {
            Task.Run(() => {
                while (!AppService.Instance.IsPoeNinjaCacheReady()) {
                    Thread.Sleep(1000);
                }

                App.Current.Dispatcher.Invoke(delegate {
                    (new StatsWindow()).Show();
                });
            });
        }

        private void btnBusy_Click(object sender, RoutedEventArgs e) {
            log.Trace("Busy button clicked");
            AudioService.Instance.PlayClick();
            vm.SendBusyWhisper((int)((Button)sender).Tag);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e) {
            log.Trace("Remove button clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.PlayerInvited) {
                vm.SendKick(id);
            } else {
                vm.RemoveOffer(id);
            }
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e) {
            log.Trace("Invite button clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (!offer.PlayerInvited) {
                vm.SendInvite(id);
            } else {
                vm.SendReInvite(id);
            }
        }

        private void grdOffer_MouseDown(object sender, MouseButtonEventArgs e) {
            log.Trace("Offer clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Grid)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer != null) {
                var shiftKeyDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                var controlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

                if (controlKeyDown && shiftKeyDown) {
                    vm.SendStillInterestedWhisper(id);
                } else if (shiftKeyDown) {
                    vm.HighlightItem(id);
                } else if (controlKeyDown) {
                    vm.SendSoldWhisper(id);
                } else {
                    switch (offer.State) {
                        case OfferState.Initial:
                            vm.SendInvite(id);
                            break;

                        case OfferState.PlayerInvited:
                        case OfferState.TradeRequestSent:
                            vm.SendTradeRequest(id);
                            break;

                    }
                }
            }
        }

        private void btnJoinHideout_Click(object sender, RoutedEventArgs e) {
            log.Trace("Join hideout button clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.Initial) {
                return;
            }

            vm.SendJoinHideoutCommand(id);
        }

        private void btnTrade_Click(object sender, RoutedEventArgs e) {
            log.Trace("Trade button clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.HideoutJoined) {
                return;
            }

            vm.SendTradeRequest(id, true);
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e) {
            log.Trace("Leave button clicked");
            AudioService.Instance.PlayClick();

            int id = (int)((Button)sender).Tag;

            vm.SendLeave(id);
        }

        private void btnClearOffers_Click(object sender, RoutedEventArgs e) {
            log.Trace("Clear offers button clicked");
            AudioService.Instance.PlayClick();

            vm.ClearOffers();
        }

        private void btnClearOutgoingOffers_Click(object sender, RoutedEventArgs e) {
            log.Trace("Clear outgoing offers button clicked");
            AudioService.Instance.PlayClick();
            vm.ClearOutgoingOffers();
        }

        private void txtSearchOutgoingOffer_TextChanged(object sender, TextChangedEventArgs e) {
            log.Trace($"Outgoing offers search bar input: {txtSearchOutgoingOffer.Text}");
            if (string.IsNullOrEmpty(txtSearchOutgoingOffer.Text)) {
                vm.ResetFilter();
            } else {
                vm.FilterOffers(txtSearchOutgoingOffer.Text);
            }
        }

        private void txtSearchOffer_TextChanged(object sender, TextChangedEventArgs e) {
            log.Trace($"Offers offers search bar input: {txtSearchOutgoingOffer.Text}");
            if (string.IsNullOrEmpty(txtSearchOffer.Text)) {
                vm.ResetFilter(false);
            } else {
                vm.FilterOffers(txtSearchOffer.Text, false);
            }
        }

        private void itStats_Click(object sender, RoutedEventArgs e) {
            ShowStatsWindow();
        }

        private void itSettings_Click(object sender, RoutedEventArgs e) {
            log.Trace("Config system tray menu item clicked");
            vm.ShowConfigWindow();
        }

        private void itQuit_Click(object sender, RoutedEventArgs e) {
            log.Trace("Quit system tray menu item clicked");
            Application.Current.Shutdown(0);
        }

        private void trayIcon_TrayContextMenuOpen(object sender, RoutedEventArgs e) {
            vm.Notify("CurrentLeague");
        }

        private void grdChaosRecipe_MouseEnter(object sender, MouseEventArgs e) {
            grdChaosRecipe.Opacity = 0.1;
        }

        private void grdChaosRecipe_MouseLeave(object sender, MouseEventArgs e) {
            grdChaosRecipe.Opacity = 0.7;
        }

        private void grdOffers_MouseDown(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            DragStart = e.GetPosition(winOverlay);
            DragStartOffet = new Vector(grdOffers_tt.X, grdOffers_tt.Y);
            grdOffers.CaptureMouse();
        }

        private void grdOffers_MouseMove(object sender, MouseEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            if (grdOffers.IsMouseCaptured) {
                Vector offset = Point.Subtract(e.GetPosition(winOverlay), DragStart);

                grdOffers_tt.X = DragStartOffet.X + offset.X;
                grdOffers_tt.Y = DragStartOffet.Y + offset.Y;
            }
        }

        private void grdOffers_MouseUp(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            grdOffers.ReleaseMouseCapture();
        }

        private void grdIncomingControls_MouseDown(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            DragStart = e.GetPosition(winOverlay);
            DragStartOffet = new Vector(grdIncomingControls_tt.X, grdIncomingControls_tt.Y);
            grdIncomingControls.CaptureMouse();
        }

        private void grdIncomingControls_MouseUp(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            grdIncomingControls.ReleaseMouseCapture();
        }

        private void grdIncomingControls_MouseMove(object sender, MouseEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            if (grdIncomingControls.IsMouseCaptured) {
                Vector offset = Point.Subtract(e.GetPosition(winOverlay), DragStart);

                grdIncomingControls_tt.X = DragStartOffet.X + offset.X;
                grdIncomingControls_tt.Y = DragStartOffet.Y + offset.Y;
            }
        }

        private void grdOutgoingControls_MouseDown(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            DragStart = e.GetPosition(winOverlay);
            DragStartOffet = new Vector(grdOutgoingControls_tt.X, grdOutgoingControls_tt.Y);
            grdOutgoingControls.CaptureMouse();
        }

        private void grdOutgoingControls_MouseMove(object sender, MouseEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            if (grdOutgoingControls.IsMouseCaptured) {
                Vector offset = Point.Subtract(e.GetPosition(winOverlay), DragStart);

                grdOutgoingControls_tt.X = DragStartOffet.X + offset.X;
                grdOutgoingControls_tt.Y = DragStartOffet.Y + offset.Y;
            }
        }

        private void grdOutgoingControls_MouseUp(object sender, MouseButtonEventArgs e) {
            if (!vm.IsOverlayMovable) {
                return;
            }

            grdOutgoingControls.ReleaseMouseCapture();
        }

        private void btnMoveOverlay_Click(object sender, RoutedEventArgs e) {
            vm.ToggleMovableOveralay(grdOffers_tt, grdIncomingControls_tt,grdOutgoingControls_tt);
        }
    }
}
