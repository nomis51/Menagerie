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

        public ObservableCollection<Offer> Offers { get; set; } = new ObservableCollection<Offer>();
        public ObservableCollection<Offer> OutgoingOffers { get; set; } = new ObservableCollection<Offer>();

        public MainWindowViewModel() {
            Parser.Instance.Start();
            ClientFileHandler.Instance.Start();
            ChatHandler.Instance.Start();
            GameHandler.Instance.Start();
            ClipboardHandler.Instance.Start();
            PoeWindowHandler.Instance.Start();

            ClipboardHandler.Instance.OnNewClipboardText += Clipboard_OnNewClipboardText;
            ClientFileHandler.Instance.OnNewLine += ClientFile_OnNewLine;
            Parser.Instance.OnNewChatEvent += Parser_OnNewChatEvent;
            Parser.Instance.OnNewOffer += Parser_OnNewOffer;
            Parser.Instance.OnNewPlayerJoined += Parser_OnNewPlayerJoined;

            PoeWindowHandler.Instance.Focus();

            // TODO: For testing only
            ClientFileHandler.Instance.Test();
            OutgoingOffers.Add(new Offer() {
                Id = 99,
                ItemName = "Saqawal",
                Price = 9,
                Currency = "Chaos",
                PlayerName = "Paul",
                IsOutgoing = true
            });
        }

        private void ClientFile_OnNewLine(string line) {
            Parser.Instance.ParseClientLine(line);
        }

        private void Clipboard_OnNewClipboardText(string text) {
            Parser.Instance.ParseClipboardLine(text);
        }

        private void Parser_OnNewPlayerJoined(string playerName) {
            foreach (var offer in Offers) {
                if (offer.PlayerName == playerName) {
                    offer.PlayerJoined = true;
                }
            }

            UpdateOffers();
        }

        private void Parser_OnNewChatEvent(ChatEvent type) {
            switch (type) {
                case ChatEvent.TradeAccepted:
                    var offer = GetActiveOffer();

                    if (offer == null) {
                        return;
                    }

                    offer.State = OfferState.Done;

                    if (offer.IsOutgoing) {
                        SendLeave(offer.Id, true);
                    } else {
                        SendKick(offer.Id, true);
                    }
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
                OutgoingOffers.Insert(0, new Offer(offer));
            }
        }

        public Offer GetOffer(int id) {
            var offer = Offers.FirstOrDefault(e => e.Id == id);

            return offer == null ? OutgoingOffers.FirstOrDefault(e => e.Id == id) : offer;
        }

        private Offer GetActiveOffer() {
            var offer = Offers.FirstOrDefault(o => o.TradeRequestSent);

            return offer == null ? OutgoingOffers.FirstOrDefault(o => o.TradeRequestSent) : offer;
        }

        private void EnsureNotHighlighted(int index) {
            if (Offers[index].IsHighlighted) {
                Offers[index].IsHighlighted = false;
                GameHandler.Instance.Input.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.ESCAPE);
            }
        }

        private int GetOfferIndex(int id) {
            int index = Offers.Select(g => g.Id)
                .ToList()
                .IndexOf(id);

            return index == -1 ?
                OutgoingOffers.Select(g => g.Id)
                .ToList()
                .IndexOf(id) :
                index;
        }

        private void UpdateOffers() {
            Offer[] buffer = new Offer[Offers.Count];
            Offers.CopyTo(buffer, 0);
            Offers.Clear();

            foreach (var o in buffer) {
                Offers.Add(o);
            }
        }

        public void SendTradeRequest(int id, bool isOutgoing = false) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (isOutgoing) {
                if (OutgoingOffers[index].State != OfferState.HideoutJoined) {
                    return;
                }

                OutgoingOffers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                ChatHandler.Instance.SendTradeCommand(OutgoingOffers[index].PlayerName);
            } else {
                if (!Offers[index].PlayerInvited) {
                    return;
                }

                Offers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                EnsureNotHighlighted(index);

                ChatHandler.Instance.SendTradeCommand(Offers[index].PlayerName);
            }
        }

        public void SendJoinHideoutCommand(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            OutgoingOffers[index].State = OfferState.HideoutJoined;
            UpdateOffers();

            ChatHandler.Instance.SendHideoutCommand(OutgoingOffers[index].PlayerName);
        }

        public void SendBusyWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (Offers[index].State != OfferState.Initial) {
                return;
            }

            ChatHandler.Instance.SendChatMessage($"@{Offers[index].PlayerName} I'm busy right now, I'll whisper you for the \"{Offers[index].ItemName}\" when I'm ready");
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
                EnsureNotHighlighted(index);

                ChatHandler.Instance.SendKickCommand(Offers[index].PlayerName);
                Thread.Sleep(100);
                ChatHandler.Instance.SendInviteCommand(Offers[index].PlayerName);
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

            EnsureNotHighlighted(index);

            ChatHandler.Instance.SendInviteCommand(Offers[index].PlayerName);
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
                ChatHandler.Instance.SendKickCommand(playerName);

                if (sayThanks) {
                    Thread.Sleep(250);
                    ChatHandler.Instance.SendChatMessage($"@{playerName} Thank you and have fun!");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            RemoveOffer(id);
        }

        public void SendLeave(int id, bool sayThanks = false) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            OutgoingOffers[index].State = OfferState.Done;
            UpdateOffers();

            string playerName = OutgoingOffers[index].PlayerName;

            Thread t = new Thread(delegate () {
                // TODO: Kick myself here

                if (sayThanks) {
                    Thread.Sleep(100);
                    ChatHandler.Instance.SendChatMessage($"@{playerName} Thank you and have fun!");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            RemoveOffer(id, true);
        }


        public void RemoveOffer(int id, bool isOutgoing = false) {
            int index = isOutgoing ? OutgoingOffers.Select(e => e.Id)
                .ToList()
                .IndexOf(id) : Offers.Select(e => e.Id)
                .ToList()
                .IndexOf(id);

            if (index != -1) {
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    (isOutgoing ? OutgoingOffers : Offers).RemoveAt(index);
                    UpdateOffers();
                    PoeWindowHandler.Instance.Focus();
                });
            }
        }

        public void SendStillInterestedWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            UpdateOffers();

            EnsureNotHighlighted(index);

            ChatHandler.Instance.SendChatMessage($"@{Offers[index].PlayerName} Are you still interested in my \"{Offers[index].ItemName}\" listed for {Offers[index].Price} {Offers[index].Currency}?");
        }

        public void SendSoldWhisper(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            EnsureNotHighlighted(index);

            ChatHandler.Instance.SendChatMessage($"@{Offers[index].PlayerName} I'm sorry, my \"{Offers[index].ItemName}\" has already been sold");

            RemoveOffer(id);
        }

        public void ClearOffers() {
            PoeWindowHandler.Instance.Focus();
            Offers.Clear();
        }

        public void ClearOutgoingOffers() {
            PoeWindowHandler.Instance.Focus();
            OutgoingOffers.Clear();
        }

        public void HighlightItem(int id) {
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].IsHighlighted = true;

            GameHandler.Instance.HightlightStash(Offers[index].ItemName);
        }
    }
}
