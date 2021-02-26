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
using Menagerie.Services;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;
using Windows.UI.Notifications;
using System.Xml;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Media;

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

        private Queue<Offer> OverflowOffers = new Queue<Offer>();
        private Queue<Offer> OverflowOutgoingOffers = new Queue<Offer>();

        public ObservableCollection<Offer> Offers { get; set; } = new ObservableCollection<Offer>();
        public ObservableCollection<Offer> OutgoingOffers { get; set; } = new ObservableCollection<Offer>();

        CoreModels.PoeApi.Stash.ChaosRecipeResult _chaosRecipe = new CoreModels.PoeApi.Stash.ChaosRecipeResult();
        public CoreModels.PoeApi.Stash.ChaosRecipeResult ChaosRecipe {
            get {
                return _chaosRecipe;
            }
            set {
                _chaosRecipe = value;
                OnPropertyChanged("ChaosRecipe");
                OnPropertyChanged("GlovesVisible");
                OnPropertyChanged("BootsVisible");
                OnPropertyChanged("HelmetsVisible");
                OnPropertyChanged("BeltsVisible");
                OnPropertyChanged("BodyArmoursVisible");
                OnPropertyChanged("RingsVisible");
                OnPropertyChanged("AmuletsVisible");
                OnPropertyChanged("WeaponsVisible");
            }
        }

        public int ChaosRecipeGridWidth {
            get {
                return DockChaosRecipeOverlayVisible == Visibility.Visible ? 530 : 60;
            }
        }

        public int ChaosRecipeGridHeight {
            get {
                return DockChaosRecipeOverlayVisible == Visibility.Visible ? 40 : 380;
            }
        }

        private Visibility _stackChaosRecipeOverlayVisible = Visibility.Hidden;
        public Visibility StackChaosRecipeOverlayVisible {
            get {
                return _stackChaosRecipeOverlayVisible;
            }
            set {
                _stackChaosRecipeOverlayVisible = value;
                OnPropertyChanged("StackChaosRecipeOverlayVisible");
                OnPropertyChanged("ChaosRecipeGridHeight");
                OnPropertyChanged("ChaosRecipeGridWidth");
            }
        }

        private Visibility _dockChaosRecipeOverlayVisible = Visibility.Visible;
        public Visibility DockChaosRecipeOverlayVisible {
            get {
                return _dockChaosRecipeOverlayVisible;
            }
            set {
                _dockChaosRecipeOverlayVisible = value;
                OnPropertyChanged("DockChaosRecipeOverlayVisible");
                OnPropertyChanged("ChaosRecipeGridHeight");
                OnPropertyChanged("ChaosRecipeGridWidth");
            }
        }


        private Visibility _chaosRecipeOverlayVisible = Visibility.Collapsed;
        public Visibility ChaosRecipeOverlayVisible {
            get {
                var config = AppService.Instance.GetConfig();
                var state = _chaosRecipeOverlayVisible == Visibility.Collapsed ? (config != null && config.ChaosRecipeEnabled ? Visibility.Visible : Visibility.Hidden) : _chaosRecipeOverlayVisible;
                return state;
            }
            set {
                _chaosRecipeOverlayVisible = value;
                OnPropertyChanged("ChaosRecipeOverlayVisible");
            }
        }

        public Visibility GlovesVisible {
            get {
                return ChaosRecipe.NeedGloves ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility BootsVisible {
            get {
                return ChaosRecipe.NeedBoots ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility HelmetsVisible {
            get {
                return ChaosRecipe.NeedHelmets ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility BodyArmoursVisible {
            get {
                return ChaosRecipe.NeedBodyArmours ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility RingsVisible {
            get {
                return ChaosRecipe.NeedRings ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility AmuletsVisible {
            get {
                return ChaosRecipe.NeedAmulets ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility BeltsVisible {
            get {
                return ChaosRecipe.NeedBelts ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public Visibility WeaponsVisible {
            get {
                return ChaosRecipe.NeedWeapons ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public string AppVersion {
            get {
                return $"Version {GetAppVersion()}";
            }
        }

        public Icon MenagerieIcon {
            get {
                return Properties.Resources.menagerie_logo;
            }
        }

        public string CurrentLeague {
            get {
                var config = AppService.Instance.GetConfig();
                return $"League: {(config != null ? config.CurrentLeague : "Standard")}";
            }
        }

        private string GetAppVersion() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";
            AppService.SetAppVersion(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);
            return version;
        }

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


        private bool _isOverlayMovable = false;
        public bool IsOverlayMovable {
            get {
                return _isOverlayMovable;
            }
        }

        public Brush IncomingOffersGridColor {
            get {
                return _isOverlayMovable ? Brushes.Blue : Brushes.Transparent;
            }
        }

        public Brush OutgoingOffersGridColor {
            get {
                return _isOverlayMovable ? Brushes.Green : Brushes.Transparent;
            }
        }

        public Brush IncomingOffersControlsGridColor {
            get {
                return _isOverlayMovable ? Brushes.Red : Brushes.Transparent;
            }
        }

        public OverlayViewModel() {
            log.Trace("Initializing OverlayViewModel");
            AppService.Instance.OnNewOffer += AppService_OnNewOffer;
            AppService.Instance.OnNewChatEvent += AppService_OnNewChatEvent;
            AppService.Instance.OnNewPlayerJoined += AppService_OnNewPlayerJoined;
            AppService.Instance.OnOfferScam += Instance_OnOfferScam;
            AppService.Instance.OnNewTradeChatLine += AppService_OnNewTradeChatLine;
            AppService.Instance.OnNewChaosRecipeResult += AppService_OnNewChaosRecipeResult;
            AppService.Instance.OnToggleChaosRecipeOverlayVisibility += AppService_OnToggleChaosRecipeOverlayVisibility;
        }

        private void AppService_OnToggleChaosRecipeOverlayVisibility(bool show) {
            ChaosRecipeOverlayVisible = show ? Visibility.Visible : Visibility.Hidden;
        }

        private void AppService_OnNewChaosRecipeResult(CoreModels.PoeApi.Stash.ChaosRecipeResult result) {
            ChaosRecipe = result;
        }

        public void Notify(string name) {
            OnPropertyChanged(name);
        }

        private void AppService_OnNewTradeChatLine(CoreModels.TradeChatLine line) {
            NotificationService.Instance.ShowTradeChatMatchNotification(line);
        }

        private void Instance_OnOfferScam(CoreModels.PriceCheckResult result, CoreModels.Offer offer) {
            int nbTry = 0;

            while (++nbTry <= 5) {
                foreach (var o in Offers) {
                    if (o.Id == offer.Id) {
                        App.Current.Dispatcher.Invoke(delegate {
                            o.PriceCheck = result;
                            o.PossibleScam = true;
                            UpdateOffers();
                        });
                        return;
                    }
                }

                Thread.Sleep(200);
            }
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

            AudioService.Instance.PlayKnock();

            App.Current.Dispatcher.Invoke(delegate {
                foreach (var offer in Offers) {
                    if (offer.PlayerName == playerName) {
                        log.Trace($"player \"{playerName}\" joined");
                        offer.PlayerJoined = true;
                    }
                }

                UpdateOffers();
            });
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
                            AppService.Instance.OfferCompleted(new CoreModels.Offer() {
                                ItemName = offer.ItemName,
                                Currency = offer.Currency,
                                Price = offer.Price,
                                Time = offer.Time,
                                League = offer.League,
                                PlayerName = offer.PlayerName,
                                EvenType = ChatEventEnum.Offer,
                                IsOutgoing = offer.IsOutgoing,
                                Id = offer.Id,
                                StashTab = offer.StashTab,
                                Position = offer.Position,
                                Notes = offer.Notes
                            });

                            SendKick(offer.Id, AppService.Instance.GetConfig().AutoThanks);
                        }
                        break;

                    case ChatEventEnum.TradeCancelled:
                        foreach (var o in Offers) {
                            if (o.TradeRequestSent) {
                                o.State = OfferState.PlayerInvited;
                            }
                        }

                        UpdateOffers();
                        break;
                }
            });
        }

        public void SetOverlayHandle(IntPtr handle) {
            AppService.Instance.SetOverlayHandle(handle);
        }

        public void UpdateElapsedTime() {
            UpdateOffers();
            OnPropertyChanged("Tooltip");
        }

        private void AppService_OnNewOffer(Core.Models.Offer offer) {
            log.Trace("New offer event");
            var config = Config;

            if (config.OnlyShowOffersOfCurrentLeague && config.CurrentLeague != offer.League) {
                return;
            }

            if (!offer.IsOutgoing) {
                AudioService.Instance.PlayNotif1();
            }

            App.Current.Dispatcher.Invoke(delegate {
                if (!offer.IsOutgoing) {
                    if (Offers.Count >= 8) {
                        OverflowOffers.Enqueue(new Offer(offer));
                    } else {
                        Offers.Add(new Offer(offer));
                    }
                } else {
                    if (OutgoingOffers.Count >= 8) {
                        var buffer = OutgoingOffers.ToList();
                        OverflowOutgoingOffers.Enqueue(buffer.Last());
                        buffer.RemoveAt(buffer.Count - 1);
                        OutgoingOffers.Clear();
                        buffer.ForEach(o => OutgoingOffers.Add(o));
                        OutgoingOffers.Add(new Offer(offer));
                        ReorderOutgoingOffers();
                    } else {
                        OutgoingOffers.Add(new Offer(offer));
                        ReorderOutgoingOffers();
                    }
                }

                OnPropertyChanged("IsOffersFilterVisible");
                OnPropertyChanged("IsOutgoingOffersFilterVisible");
            });
        }

        private void ReorderOutgoingOffers() {
            var buffer = OutgoingOffers.ToList()
                .OrderByDescending(o => o.Time);

            OutgoingOffers.Clear();

            foreach (var o in buffer) {
                OutgoingOffers.Add(o);
            }
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

        private void EnsureNotHighlighted(int index, bool isOutgoing = false) {
            //log.Trace("Verify not highlighted");

            //if (isOutgoing) {
            //    if (OutgoingOffers[index].IsHighlighted) {
            //        OutgoingOffers[index].IsHighlighted = false;
            //        AppService.Instance.FocusGame();
            //        AppService.Instance.ClearSpecialKeys();
            //        AppService.Instance.EnsureNotHighlightingItem();
            //        Thread.Sleep(100);
            //    }
            //} else {
            //    if (Offers[index].IsHighlighted) {
            //        Offers[index].IsHighlighted = false;
            //        AppService.Instance.FocusGame();
            //        AppService.Instance.ClearSpecialKeys();
            //        AppService.Instance.EnsureNotHighlightingItem();
            //        Thread.Sleep(100);
            //    }
            //}
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

                // EnsureNotHighlighted(index);

                AppService.Instance.SendTradeChatCommand(OutgoingOffers[index].PlayerName);
            } else {
                if (!Offers[index].PlayerInvited) {
                    return;
                }

                Offers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                //   EnsureNotHighlighted(index, true);

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

            //   EnsureNotHighlighted(index, true);

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

            //    EnsureNotHighlighted(index);

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().BusyWhisper, new CoreModels.Offer() { ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League })}");
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
                //     EnsureNotHighlighted(index);

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

            //    EnsureNotHighlighted(index);

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
                //  EnsureNotHighlighted(index);
                AppService.Instance.SendKickChatCommand(playerName);

                if (sayThanks) {
                    Thread.Sleep(250);
                    AppService.Instance.SendChatMessage($"@{playerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().ThanksWhisper, new CoreModels.Offer() { ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League })}");
                }

                RemoveOffer(id);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
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
                //    EnsureNotHighlighted(index);
                var config = AppService.Instance.GetConfig();

                if (!string.IsNullOrEmpty(config.PlayerName)) {
                    AppService.Instance.SendKickChatCommand(config.PlayerName);
                }

                if (sayThanks) {
                    Thread.Sleep(100);
                    AppService.Instance.SendChatMessage($"@{playerName} {(AppService.Instance.ReplaceVars(config.ThanksWhisper, new CoreModels.Offer() { ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League }))}");
                }

                RemoveOffer(id, true);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void RemoveOffer(int id, bool isOutgoing = false) {
            log.Trace($"Removing offer {id}");
            int index = isOutgoing ? OutgoingOffers.Select(e => e.Id)
                .ToList()
                .IndexOf(id) : Offers.Select(e => e.Id)
                .ToList()
                .IndexOf(id);

            if (index != -1) {
                App.Current.Dispatcher.Invoke(() => {
                    var refOffers = (isOutgoing ? OutgoingOffers : Offers);
                    refOffers.RemoveAt(index);

                    if (refOffers.Count < 8) {
                        if (isOutgoing) {
                            if (OverflowOutgoingOffers.Count > 0) {
                                OutgoingOffers.Add(OverflowOutgoingOffers.Dequeue());
                                ReorderOutgoingOffers();
                            }
                        } else {
                            if (OverflowOffers.Count > 0) {
                                Offers.Add(OverflowOffers.Dequeue());
                            }
                        }
                    }

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

            //    EnsureNotHighlighted(index);

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().StillInterestedWhisper, new CoreModels.Offer() { ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League })}");
        }

        public void SendSoldWhisper(int id) {
            log.Trace($"Sending sold whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            //    EnsureNotHighlighted(index);

            AppService.Instance.SendChatMessage($"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().SoldWhisper, new CoreModels.Offer() { ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League })}");

            var offer = Offers[index];
            AppService.Instance.OfferCompleted(new CoreModels.Offer() {
                ItemName = offer.ItemName,
                Currency = offer.Currency,
                Price = offer.Price,
                Time = offer.Time,
                League = offer.League,
                PlayerName = offer.PlayerName,
                EvenType = ChatEventEnum.Offer,
                IsOutgoing = offer.IsOutgoing,
                Id = offer.Id
            });

            RemoveOffer(id);
        }

        public void ClearOffers() {
            log.Trace("Clearing offers");
            AppService.Instance.FocusGame();
            Offers.Clear();


            while (OverflowOffers.Count > 0) {
                Offers.Add(OverflowOffers.Dequeue());
            }

            OnPropertyChanged("IsOffersFilterVisible");
        }

        public void ClearOutgoingOffers() {
            log.Trace("Clearing outgoing offers");
            AppService.Instance.FocusGame();
            OutgoingOffers.Clear();

            while (OverflowOutgoingOffers.Count > 0) {
                OutgoingOffers.Add(OverflowOutgoingOffers.Dequeue());
            }

            ReorderOutgoingOffers();

            OnPropertyChanged("IsOutgoingOffersFilterVisible");
        }

        public void HighlightItem(int id) {
            log.Trace($"Highlighting offer {id}");
            var index = GetOfferIndex(id);

            if (index == -1) {
                return;
            }

            Offers[index].IsHighlighted = true;

            AppService.Instance.HightlightStash(Offers[index].EscapedName);
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

        public void ToggleMovableOveralay(TranslateTransform grdOffers, TranslateTransform grdOffersControls, TranslateTransform grdOutgoingOffers, TranslateTransform grdChaosRecipe, bool chaosRecipeDockMode = true) {
            _isOverlayMovable = !_isOverlayMovable;
            OnPropertyChanged("IncomingOffersGridColor");
            OnPropertyChanged("IncomingOffersControlsGridColor");
            OnPropertyChanged("OutgoingOffersGridColor");

            if (!_isOverlayMovable) {
                var config = Config;
                config.IncomingOffersGridOffset = new System.Drawing.Point((int)grdOffers.X, (int)grdOffers.Y);
                config.IncomingOffersControlsGridOffset = new System.Drawing.Point((int)grdOffersControls.X, (int)grdOffersControls.Y);
                config.OutgoingOffersGridOffset = new System.Drawing.Point((int)grdOutgoingOffers.X, (int)grdOutgoingOffers.Y);
                config.ChaosRecipeGridOffset = new System.Drawing.Point((int)grdChaosRecipe.X, (int)grdChaosRecipe.Y);
                config.ChaosRecipeOveralyDockMode = chaosRecipeDockMode;
                AppService.Instance.SetConfig(config);
            }
        }
    }
}
