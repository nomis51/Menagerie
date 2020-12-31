using Menagerie.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using Menagerie.Core.Services;
using Menagerie.Core.Enums;
using CoreModels = Menagerie.Core.Models;
using Menagerie.Views;
using log4net;
using Menagerie.Core.Extensions;

namespace Menagerie.ViewModels {
    public class OverlayViewModel : INotifyPropertyChanged {
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

        private readonly static ILog log = LogManager.GetLogger(typeof(OverlayViewModel));

        private ConfigWindow ConfigWin;

        private Offer[] _offers;
        private Offer[] _outgoingOffers;

        public ObservableCollection<Offer> Offers { get; set; } = new ObservableCollection<Offer>();
        public ObservableCollection<Offer> OutgoingOffers { get; set; } = new ObservableCollection<Offer>();

        public Visibility IsOffersFilterVisible {
            get {
                return Offers.Count > 1 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility IsOutgoingOffersFilterVisible {
            get {
                return OutgoingOffers.Count > 1 ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public CoreModels.Config Config {
            get {
                return AppService.Instance.GetConfig();
            }
        }

        public OverlayViewModel() {
            log.Trace("Initializing OverlayViewModel");
            AppService.Instance.OnNewOffer += AppService_OnNewOffer;
            AppService.Instance.OnNewChatEvent += AppService_OnNewChatEvent;
            AppService.Instance.OnNewPlayerJoined += AppService_OnNewPlayerJoined;
        }

        public void ShowConfigWindow() {
            log.Trace("Showing config window");
            ConfigWin = new ConfigWindow();
            ConfigWin.Closed += ConfigWin_Closed;
            ConfigWin.Show();
        }

        private void ConfigWin_Closed(object sender, EventArgs e) {
            log.Trace("Deleting config window");
            ConfigWin.Closed -= ConfigWin_Closed;
            ConfigWin = null;
        }

        private void AppService_OnNewPlayerJoined(string playerName) {
            log.Trace("New player joined event");

            App.Current.Dispatcher.Invoke(delegate {
                foreach (var offer in Offers) {
                    if (offer.PlayerName == playerName) {
                        log.Trace($"player \"{playerName}\" joined");
                        offer.PlayerJoined = true;
                    }
                }
            });

            UpdateOffers();
        }

        private void AppService_OnNewChatEvent(ChatEventEnum type) {
            log.Trace($"New chat event: {type.ToString()}");

            App.Current.Dispatcher.Invoke(delegate {
                switch (type) {
                    case ChatEventEnum.TradeAccepted:
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

                    case ChatEventEnum.TradeCancelled:
                        foreach (var o in Offers) {
                            if (o.TradeRequestSent) {
                                o.State = OfferState.PlayerInvited;
                            }
                        }
                        break;
                }
            });
        }

        private void AppService_OnNewOffer(Core.Models.Offer offer) {
            log.Trace("New offer event");
            var config = Config;

            if (config.OnlyShowOffersOfCurrentLeague && config.CurrentLeague != offer.League) {
                return;
            }

            App.Current.Dispatcher.Invoke(delegate {
                if (!offer.IsOutgoing) {
                    Offers.Add(new Offer(offer));
                } else {
                    OutgoingOffers.Insert(0, new Offer(offer));
                }

                OnPropertyChanged("IsOffersFilterVisible");
                OnPropertyChanged("IsOutgoingOffersFilterVisible");
            });
        }

        public List<string> GetLeagues() {
            log.Trace("Getting leagues");
            return AppService.Instance.GetLeagues().Result;
        }

        public Offer GetOffer(int id) {
            log.Trace($"Getting offer {id}");
            var offer = Offers.FirstOrDefault(e => e.Id == id);

            return offer == null ? OutgoingOffers.FirstOrDefault(e => e.Id == id) : offer;
        }

        private Offer GetActiveOffer() {
            log.Trace("Getting active offer");
            var offer = Offers.FirstOrDefault(o => o.TradeRequestSent);

            return offer == null ? OutgoingOffers.FirstOrDefault(o => o.TradeRequestSent) : offer;
        }

        private void EnsureNotHighlighted(int index) {
            log.Trace("Verify not highlighted");
            if (Offers[index].IsHighlighted) {
                Offers[index].IsHighlighted = false;
                AppService.Instance.SendEscape();
            }
        }

        private int GetOfferIndex(int id) {
            log.Trace($"Getting offer's index {id}");
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
            log.Trace("Updating offers");
            Offer[] buffer = new Offer[Offers.Count];
            Offers.CopyTo(buffer, 0);
            Offers.Clear();

            foreach (var o in buffer) {
                Offers.Add(o);
            }

            Offer[] buffer2 = new Offer[OutgoingOffers.Count];
            OutgoingOffers.CopyTo(buffer2, 0);
            OutgoingOffers.Clear();

            foreach (var o in buffer2) {
                OutgoingOffers.Add(o);
            }

            OnPropertyChanged("IsOffersFilterVisible");
            OnPropertyChanged("IsOutgoingOffersFilterVisible");
        }

        public void SendTradeRequest(int id, bool isOutgoing = false) {
            log.Trace($"Sending trade request {id}");
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

                AppService.Instance.SendTradeChatCommand(OutgoingOffers[index].PlayerName);
            } else {
                if (!Offers[index].PlayerInvited) {
                    return;
                }

                Offers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                EnsureNotHighlighted(index);

                AppService.Instance.SendTradeChatCommand(Offers[index].PlayerName);
            }
        }

        public void SendJoinHideoutCommand(int id) {
            log.Trace($"Sending join hideout command {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            OutgoingOffers[index].State = OfferState.HideoutJoined;
            UpdateOffers();

            AppService.Instance.SendHideoutChatCommand(OutgoingOffers[index].PlayerName);
        }

        public void SendBusyWhisper(int id) {
            log.Trace($"Sending busy whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (Offers[index].State != OfferState.Initial) {
                return;
            }

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} I'm busy right now, I'll whisper you for the \"{Offers[index].ItemName}\" when I'm ready");
        }

        public void SendReInvite(int id) {
            log.Trace($"Sending re-invite commands {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            if (!Offers[index].PlayerInvited) {
                return;
            }

            Thread t = new Thread(delegate () {
                EnsureNotHighlighted(index);

                AppService.Instance.SendKickChatCommand(Offers[index].PlayerName);
                Thread.Sleep(100);
                AppService.Instance.SendInviteChatCommand(Offers[index].PlayerName);
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendInvite(int id) {
            log.Trace($"Sending invite command {id}");
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

            AppService.Instance.SendInviteChatCommand(Offers[index].PlayerName);
        }

        public void SendKick(int id, bool sayThanks = false) {
            log.Trace($"Sending kick command {id}");
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
                AppService.Instance.SendKickChatCommand(playerName);

                if (sayThanks) {
                    Thread.Sleep(250);
                    AppService.Instance.SendChatMessage($"@{playerName} Thank you and have fun!");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            RemoveOffer(id);
        }

        public void SendLeave(int id, bool sayThanks = false) {
            log.Trace($"Sending leave command {id}");
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
                    AppService.Instance.SendChatMessage($"@{playerName} Thank you and have fun!");
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

            RemoveOffer(id, true);
        }

        public void RemoveOffer(int id, bool isOutgoing = false) {
            log.Trace($"Removing offer {id}");
            int index = isOutgoing ? OutgoingOffers.Select(e => e.Id)
                .ToList()
                .IndexOf(id) : Offers.Select(e => e.Id)
                .ToList()
                .IndexOf(id);

            if (index != -1) {
                Dispatcher.CurrentDispatcher.Invoke(() => {
                    (isOutgoing ? OutgoingOffers : Offers).RemoveAt(index);
                    UpdateOffers();
                    AppService.Instance.FocusGame();
                });
            }
        }

        public void SendStillInterestedWhisper(int id) {
            log.Trace($"Sending still interested whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            UpdateOffers();

            EnsureNotHighlighted(index);

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} Are you still interested in my \"{Offers[index].ItemName}\" listed for {Offers[index].Price} {Offers[index].Currency}?");
        }

        public void SendSoldWhisper(int id) {
            log.Trace($"Sending sold whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            EnsureNotHighlighted(index);

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} I'm sorry, my \"{Offers[index].ItemName}\" has already been sold");

            RemoveOffer(id);
        }

        public void ClearOffers() {
            log.Trace("Clearing offers");
            AppService.Instance.FocusGame();
            Offers.Clear();
            OnPropertyChanged("IsOffersFilterVisible");
        }

        public void ClearOutgoingOffers() {
            log.Trace("Clearing outgoing offers");
            AppService.Instance.FocusGame();
            OutgoingOffers.Clear();
            OnPropertyChanged("IsOutgoingOffersFilterVisible");
        }

        public void HighlightItem(int id) {
            log.Trace($"Highlighting offer {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].IsHighlighted = true;

            AppService.Instance.HightlightStash(Offers[index].ItemName);
        }

        public void ResetFilter(bool applyToOutgoing = true) {
            log.Trace($"Resetting filter {(applyToOutgoing ? "Outgoing" : "Incoming")} offers");
            if (applyToOutgoing) {
                if (_outgoingOffers != null) {
                    OutgoingOffers.Clear();

                    foreach (var offer in _outgoingOffers) {
                        OutgoingOffers.Add(offer);
                    }
                }
            } else {
                if (_offers != null) {
                    Offers.Clear();

                    foreach (var offer in _offers) {
                        Offers.Add(offer);
                    }
                }
            }
        }

        public void FilterOffers(string searchText, bool applyToOutgoing = true) {
            log.Trace($"Filtering {(applyToOutgoing ? "Outgoing" : "Incoming")} offers with {searchText}");

            searchText = searchText.ToLower().Trim();

            ResetFilter(applyToOutgoing);

            if (applyToOutgoing) {
                var results = OutgoingOffers.ToList().FindAll(e => e.ItemName.ToLower().IndexOf(searchText) != -1 || e.PlayerName.ToLower().IndexOf(searchText) != -1);

                if (_outgoingOffers == null) {
                    _outgoingOffers = new Offer[OutgoingOffers.Count];
                }

                OutgoingOffers.CopyTo(_outgoingOffers, 0);
                OutgoingOffers.Clear();

                foreach (var r in results) {
                    OutgoingOffers.Add(r);
                }
            } else {
                var results = Offers.ToList().FindAll(e => e.ItemName.ToLower().IndexOf(searchText) != -1 || e.PlayerName.ToLower().IndexOf(searchText) != -1);

                if (_offers == null) {
                    _offers = new Offer[Offers.Count];
                }

                Offers.CopyTo(_offers, 0);
                Offers.Clear();

                foreach (var r in results) {
                    Offers.Add(r);
                }
            }
        }

        public void SetCurrentLeague(string league) {
            log.Trace($"Setting current to {league}");
            var config = Config;
            config.CurrentLeague = league;
            AppService.Instance.SetConfig(new Core.Models.Config() {
                Id = config.Id,
                CurrentLeague = config.CurrentLeague,
                OnlyShowOffersOfCurrentLeague = config.OnlyShowOffersOfCurrentLeague,
                PlayerName = config.PlayerName
            });
        }

        public string GetCurrentLeague() {
            log.Trace("Getting current league");
            return Config == null ? "" : Config.CurrentLeague;
        }
    }
}
