using log4net;
using Menagerie.Core.Services;
using Menagerie.Models;
using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using Menagerie.Core.Extensions;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Menagerie {
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class OverlayWindow : Window {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        private static readonly ILog log = LogManager.GetLogger(typeof(OverlayWindow));

        private readonly System.Drawing.Rectangle screenRect;

        public OverlayViewModel vm;
        private Forms.NotifyIcon trayIcon = null;


        public OverlayWindow(Forms.Screen screen) {
            InitializeComponent();

            log.Trace("Initializing Overlay");

            screenRect = screen.WorkingArea;

            vm = new OverlayViewModel();
            this.DataContext = vm;

            this.SourceInitialized += OverlayWindow_SourceInitialized;
            this.Loaded += OverlayWindow_Loaded;

            SetupTrayIcon();

            AppService.Instance.OnToggleOverlayVisibility += AppService_OnToggleOverlayVisibility;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Maximized;
        }

        private void OverlayWindow_SourceInitialized(object sender, EventArgs e) {
            base.OnSourceInitialized(e);
            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;
            MoveWindow(hWnd, screenRect.Left, screenRect.Top, screenRect.Width, screenRect.Height, false);
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

        private string GetAppVersion() {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private void SetupTrayIcon() {
            log.Trace("Initializing system tray icon");
            trayIcon = new Forms.NotifyIcon();
            trayIcon.Icon = Properties.Resources.menagerie_logo;

            Forms.ContextMenuStrip menu = new Forms.ContextMenuStrip();

            Forms.ToolStripMenuItem versionItem = new Forms.ToolStripMenuItem() {
                Text = $"Version {GetAppVersion()}",
                Enabled = false
            };

            Forms.ToolStripMenuItem leagueItem = new Forms.ToolStripMenuItem() {
                Text = "League"
            };

            List<string> leagues = vm.GetLeagues();
            string currentLeague = vm.GetCurrentLeague();

            foreach (var l in leagues) {
                var item = new Forms.ToolStripMenuItem() {
                    Text = l,
                    Checked = currentLeague == l
                };
                item.Click += LeagueMenuItem_Click;

                leagueItem.DropDownItems.Add(item);
            }

            Forms.ToolStripMenuItem configItem = new Forms.ToolStripMenuItem() {
                Text = "Settings"
            };
            configItem.Click += ConfigItem_Click;

            Forms.ToolStripMenuItem quitItem = new Forms.ToolStripMenuItem() {
                Text = "Quit"
            };
            quitItem.Click += QuitItem_Click;

            menu.Items.Add(versionItem);
            menu.Items.Add(leagueItem);
            menu.Items.Add(configItem);
            menu.Items.Add(quitItem);

            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
        }

        private void LeagueMenuItem_Click(object sender, EventArgs e) {
            log.Trace($"League system tray menu item clicked {((Forms.ToolStripMenuItem)sender).Text}");

            foreach (var i in trayIcon.ContextMenuStrip.Items) {
                if (((Forms.ToolStripMenuItem)i).Text == "League") {
                    foreach (var k in ((Forms.ToolStripMenuItem)i).DropDownItems) {
                        ((Forms.ToolStripMenuItem)k).Checked = false;
                    }
                }
            }

            Forms.ToolStripMenuItem item = (Forms.ToolStripMenuItem)sender;
            item.Checked = true;
            vm.SetCurrentLeague(item.Text);
        }

        private void ConfigItem_Click(object sender, EventArgs e) {
            log.Trace("Config system tray menu item clicked");
            vm.ShowConfigWindow();
        }

        private void QuitItem_Click(object sender, EventArgs e) {
            log.Trace("Quit system tray menu item clicked");
            Application.Current.Shutdown(0);
        }

        private void btnBusy_Click(object sender, RoutedEventArgs e) {
            log.Trace("Busy button clicked");
            vm.SendBusyWhisper((int)((Button)sender).Tag);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e) {
            log.Trace("Remove button clicked");
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
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.Initial) {
                return;
            }

            vm.SendJoinHideoutCommand(id);
        }

        private void btnTrade_Click(object sender, RoutedEventArgs e) {
            log.Trace("Trade button clicked");
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.HideoutJoined) {
                return;
            }

            vm.SendTradeRequest(id, true);
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e) {
            log.Trace("Leave button clicked");
            int id = (int)((Button)sender).Tag;

            vm.SendLeave(id);
        }

        private void btnClearOffers_Click(object sender, RoutedEventArgs e) {
            log.Trace("Clear offers button clicked");
            vm.ClearOffers();
        }

        private void btnClearOutgoingOffers_Click(object sender, RoutedEventArgs e) {
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
    }
}
