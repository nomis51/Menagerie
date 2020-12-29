using Menagerie.Core;
using Menagerie.Models;
using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;

namespace Menagerie {
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class Overlay : Window {
        private OverlayViewModel vm;
        private Forms.NotifyIcon trayIcon = null;

        public Overlay() {
            InitializeComponent();

            vm = new OverlayViewModel();
            this.DataContext = vm;

            SetupTrayIcon();
        }

        private string GetAppVersion() {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        private void SetupTrayIcon() {
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
                Text = "Config"
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
            // TODO: show config window (well I've to build it aswell lol)
        }

        private void QuitItem_Click(object sender, EventArgs e) {
            Application.Current.Shutdown(0);
        }

        private void btnBusy_Click(object sender, RoutedEventArgs e) {
            vm.SendBusyWhisper((int)((Button)sender).Tag);
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e) {
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.PlayerInvited) {
                vm.SendKick(id);
            } else {
                vm.RemoveOffer(id);
            }
        }

        private void btnInvite_Click(object sender, RoutedEventArgs e) {
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (!offer.PlayerInvited) {
                vm.SendInvite(id);
            } else {
                vm.SendReInvite(id);
            }
        }

        private void grdOffer_MouseDown(object sender, MouseButtonEventArgs e) {
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
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.Initial) {
                return;
            }

            vm.SendJoinHideoutCommand(id);
        }

        private void btnTrade_Click(object sender, RoutedEventArgs e) {
            int id = (int)((Button)sender).Tag;
            var offer = vm.GetOffer(id);

            if (offer.State != OfferState.HideoutJoined) {
                return;
            }

            vm.SendTradeRequest(id, true);
        }

        private void btnLeave_Click(object sender, RoutedEventArgs e) {
            int id = (int)((Button)sender).Tag;

            vm.SendLeave(id);
        }

        private void btnClearOffers_Click(object sender, RoutedEventArgs e) {
            vm.ClearOffers();
        }

        private void btnClearOutgoingOffers_Click(object sender, RoutedEventArgs e) {
            vm.ClearOutgoingOffers();
        }

        private void txtSearchOutgoingOffer_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrEmpty(txtSearchOutgoingOffer.Text)) {
                vm.ResetFilter();
            } else {
                vm.FilterOffers(txtSearchOutgoingOffer.Text);
            }
        }

        private void txtSearchOffer_TextChanged(object sender, TextChangedEventArgs e) {
            if (string.IsNullOrEmpty(txtSearchOffer.Text)) {
                vm.ResetFilter(false);
            } else {
                vm.FilterOffers(txtSearchOffer.Text, false);
            }
        }
    }
}
