using Toucan.Core;
using Toucan.Models;
using Toucan.ViewModels;
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

namespace Toucan {
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private MainWindowViewModel vm;

        public MainWindow() {
            InitializeComponent();

            vm = new MainWindowViewModel();
            this.DataContext = vm;
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
                if ((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))) {
                    vm.SendStillInterestedWhisper(id);
                } else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) {
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
    }
}
