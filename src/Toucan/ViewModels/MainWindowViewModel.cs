using Toucan.Core;
using Toucan.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;

namespace Toucan.ViewModels {
    public class MainWindowViewModel : INotifyPropertyChanged {
        #region Updater
        private ICommand mUpdater;
        public ICommand UpdateCommand {
            get {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set {
                mUpdater = value;
            }
        }

        private class Updater : ICommand {
            #region ICommand Members  

            public bool CanExecute(object parameter) {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter) {

            }

            #endregion
        }
        #endregion

        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private ClientFileParser ClientFile;
        private PoeWindow Poe;
        private ChatHandler Chat;
        private ClipboardListener Clipboard;
        private Parser Parser;

        public ObservableCollection<Offer> Offers { get; set; } = new ObservableCollection<Offer>();
        public ObservableCollection<Offer> OutgoingOffers { get; set; } = new ObservableCollection<Offer>();

        public MainWindowViewModel() {
            this.Poe = new PoeWindow();
            Parser = new Parser();
            this.ClientFile = new ClientFileParser(@"C:\Path of Exile\logs\Client.txt");
            Chat = new ChatHandler(this.Poe);
            Clipboard = new ClipboardListener();

            Clipboard.OnNewClipboardText += Clipboard_OnNewClipboardText;
            ClientFile.OnNewLine += ClientFile_OnNewLine;
            Parser.OnNewChatEvent += Parser_OnNewChatEvent;
            Parser.OnNewOffer += Parser_OnNewOffer;
            Parser.OnNewPlayerJoined += Parser_OnNewPlayerJoined;

            ClientFile.Test();
            Poe.Focus();

            OutgoingOffers.Add(new Offer() {
                Id = 99,
                ItemName = "Saqawal",
                Price = 9,
                Currency = "Chaos",
                PlayerName = "Paul"
            });
        }

        private void ClientFile_OnNewLine(string line) {
            Parser.ParseClientLine(line);
        }

        private void Clipboard_OnNewClipboardText(string text) {
            Parser.ParseClipboardLine(text);
        }

        private void Parser_OnNewPlayerJoined(string playerName) {
            foreach (var offer in Offers) {
                if (offer.PlayerName == playerName) {
                    offer.PlayerJoined = true;
                }
            }
        }

        private void Parser_OnNewChatEvent(ChatEvent type) {
            switch (type) {
                case ChatEvent.PlayerJoined:

                    break;

                case ChatEvent.TradeAccepted:
                    var offer = GetActiveOffer();

                    if (offer == null) {
                        offer.State = OfferState.PlayerInvited;
                        return;
                    }

                    offer.State = OfferState.Done;

                    SendKick(offer.Id, true);
                    break;

                case ChatEvent.TradeCancelled:
                    foreach (var o in Offers) {
                        if (o.TradeRequestSent) {
                            o.State = OfferState.PlayerInvited;
                        }
                    }
                    break;
            }
        }

        private void Parser_OnNewOffer(Core.Models.Offer offer) {
            if (!offer.IsOutgoing) {
                Offers.Add(new Offer(offer));
            } else {
                OutgoingOffers.Add(new Offer(offer));
            }
        }

        public void Test() {
            this.ClientFile.Test();
        }

        public Offer GetOffer(int id) {
            return Offers.FirstOrDefault(e => e.Id == id);
        }

        private Offer GetActiveOffer() {
            return Offers.FirstOrDefault(o => o.TradeRequestSent);
        }

        private int GetOfferIndex(int id) {
            return Offers.Select(g => g.Id)
                .ToList()
                .IndexOf(id);
        }

        private void UpdateOffers() {
            Offer[] buffer = new Offer[Offers.Count];
            Offers.CopyTo(buffer, 0);
            Offers.Clear();

            foreach (var o in buffer) {
                Offers.Add(o);
            }
        }

        public void SendTradeRequest(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (!Offers[index].PlayerInvited) {
                return;
            }

            Offers[index].State = OfferState.TradeRequestSent;
            UpdateOffers();

            Chat.SendTradeCommand(Offers[index].PlayerName);
        }

        public void SendBusyWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (Offers[index].State != OfferState.Initial) {
                return;
            }

            Chat.SendChatMessage($"I'm busy right now, I'll whisper you for the \"{Offers[index].ItemName}\" when I'm ready");
        }

        public void SendReInvite(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (!Offers[index].PlayerInvited) {
                return;
            }

            Thread t = new Thread(delegate () {
                Chat.SendKickCommand(Offers[index].PlayerName);
                Thread.Sleep(250);
                Chat.SendInviteCommand(Offers[index].PlayerName);
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendInvite(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (Offers[index].State != OfferState.Initial) {
                return;
            }

            Offers[index].State = OfferState.PlayerInvited;
            UpdateOffers();

            Chat.SendInviteCommand(Offers[index].PlayerName);
        }

        public void SendKick(int id, bool sayThanks = false) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (Offers[index].State == OfferState.Initial) {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            string playerName = Offers[index].PlayerName;

            Thread t = new Thread(delegate () {
                Chat.SendKickCommand(playerName);

                if (sayThanks) {
                    Thread.Sleep(250);
                    Chat.SendChatMessage("Thank you and have fun!");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            RemoveOffer(id);
        }

        public void RemoveOffer(int id) {
            int index = Offers.Select(e => e.Id)
                .ToList()
                .IndexOf(id);

            if (index != -1) {
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    Offers.RemoveAt(index);
                    UpdateOffers();
                });
            }
        }

        public void SendStillInterestedWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            UpdateOffers();

            Chat.SendChatMessage($"Are you still interested in my \"{Offers[index].ItemName}\" listed for {Offers[index].Price} {Offers[index].Currency}?");
        }

        public void SendSoldWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            Chat.SendChatMessage($"I'm sorry, my \"{Offers[index].ItemName}\" has already been sold");

            RemoveOffer(id);
        }
    }
}
