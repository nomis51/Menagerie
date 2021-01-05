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

         //   SetupTrayIcon();

            AppService.Instance.OnToggleOverlayVisibility += AppService_OnToggleOverlayVisibility;
        }

        private void OverlayWindow_Activated(object sender, EventArgs e) {
            vm.SetOverlayHandle(new WindowInteropHelper(this).Handle);
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }

        private void OverlayWindow_SourceInitialized(object sender, EventArgs e) {
          //  NotificationService.Instance.Setup(lblNotification, txtNotificationTitle, txtNotificationContent);

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

        private void btnNotificationClose_Click(object sender, RoutedEventArgs e) {
            NotificationService.Instance.CloseNotification();
        }

        private void itLeague_Click(object sender, RoutedEventArgs e) {

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
    }
}
