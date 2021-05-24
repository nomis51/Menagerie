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
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Media;
using Menagerie.Core.Models.Translator;

namespace Menagerie.ViewModels
{
    public class OverlayViewModel : INotifyPropertyChanged
    {
        #region Updater

        private ICommand mUpdater;

        public ICommand UpdateCommand
        {
            get
            {
                if (mUpdater == null)
                    mUpdater = new Updater();
                return mUpdater;
            }
            set { mUpdater = value; }
        }

        private class Updater : ICommand
        {
            #region ICommand Members

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
            }

            #endregion
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private static readonly ILog Log = LogManager.GetLogger(typeof(OverlayViewModel));

        private ConfigWindow _configWin;

        private Offer[] _offers;
        private Offer[] _outgoingOffers;

        private readonly Queue<Offer> _overflowOffers = new();
        private readonly Queue<Offer> _overflowOutgoingOffers = new();

        public ObservableCollection<Offer> Offers { get; set; } = new();
        public ObservableCollection<Offer> OutgoingOffers { get; set; } = new();

        private CoreModels.PoeApi.Stash.ChaosRecipeResult _chaosRecipe = new();

        public CoreModels.PoeApi.Stash.ChaosRecipeResult ChaosRecipe
        {
            get => _chaosRecipe;
            set
            {
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

        public int ChaosRecipeGridWidth => DockChaosRecipeOverlayVisible == Visibility.Visible ? 530 : 60;

        public int ChaosRecipeGridHeight => DockChaosRecipeOverlayVisible == Visibility.Visible ? 40 : 380;

        private Visibility _stackChaosRecipeOverlayVisible = Visibility.Hidden;

        public Visibility StackChaosRecipeOverlayVisible
        {
            get => _stackChaosRecipeOverlayVisible;
            set
            {
                _stackChaosRecipeOverlayVisible = value;
                OnPropertyChanged("StackChaosRecipeOverlayVisible");
                OnPropertyChanged("ChaosRecipeGridHeight");
                OnPropertyChanged("ChaosRecipeGridWidth");
            }
        }

        private Visibility _dockChaosRecipeOverlayVisible = Visibility.Visible;

        public Visibility DockChaosRecipeOverlayVisible
        {
            get => _dockChaosRecipeOverlayVisible;
            set
            {
                _dockChaosRecipeOverlayVisible = value;
                OnPropertyChanged("DockChaosRecipeOverlayVisible");
                OnPropertyChanged("ChaosRecipeGridHeight");
                OnPropertyChanged("ChaosRecipeGridWidth");
            }
        }


        private Visibility _chaosRecipeOverlayVisible = Visibility.Collapsed;

        public Visibility ChaosRecipeOverlayVisible
        {
            get
            {
                var config = AppService.Instance.GetConfig();
                var state = _chaosRecipeOverlayVisible == Visibility.Collapsed
                    ? (config is {ChaosRecipeEnabled: true} ? Visibility.Visible : Visibility.Hidden)
                    : _chaosRecipeOverlayVisible;
                return state;
            }
            set
            {
                _chaosRecipeOverlayVisible = value;
                OnPropertyChanged("ChaosRecipeOverlayVisible");
            }
        }

        public Visibility GlovesVisible => ChaosRecipe.NeedGloves ? Visibility.Visible : Visibility.Hidden;

        public Visibility BootsVisible => ChaosRecipe.NeedBoots ? Visibility.Visible : Visibility.Hidden;

        public Visibility HelmetsVisible => ChaosRecipe.NeedHelmets ? Visibility.Visible : Visibility.Hidden;

        public Visibility BodyArmoursVisible => ChaosRecipe.NeedBodyArmours ? Visibility.Visible : Visibility.Hidden;

        public Visibility RingsVisible => ChaosRecipe.NeedRings ? Visibility.Visible : Visibility.Hidden;

        public Visibility AmuletsVisible => ChaosRecipe.NeedAmulets ? Visibility.Visible : Visibility.Hidden;

        public Visibility BeltsVisible => ChaosRecipe.NeedBelts ? Visibility.Visible : Visibility.Hidden;

        public Visibility WeaponsVisible => ChaosRecipe.NeedWeapons ? Visibility.Visible : Visibility.Hidden;

        public string AppVersion { get; set; } = $"Version {GetAppVersion()}";

        public string CurrentLeague
        {
            get
            {
                var config = AppService.Instance.GetConfig();
                return $"League: {(config != null ? config.CurrentLeague : "Standard")}";
            }
        }

        public Visibility TranslateInputControlVisible { get; set; } = Visibility.Hidden;


        public Visibility IsOffersFilterVisible => Offers.Count > 1 ? Visibility.Visible : Visibility.Hidden;

        public Visibility IsOutgoingOffersFilterVisible =>
            OutgoingOffers.Count > 1 ? Visibility.Visible : Visibility.Hidden;

        public CoreModels.Config Config => AppService.Instance.GetConfig();


        private bool _isOverlayMovable = false;

        public bool IsOverlayMovable => _isOverlayMovable;

        public Brush IncomingOffersGridColor => _isOverlayMovable ? Brushes.Blue : Brushes.Transparent;

        public Brush OutgoingOffersGridColor => _isOverlayMovable ? Brushes.Green : Brushes.Transparent;

        public Brush IncomingOffersControlsGridColor => _isOverlayMovable ? Brushes.Red : Brushes.Transparent;

        public OverlayViewModel()
        {
            Log.Trace("Initializing OverlayViewModel");
            AppService.Instance.OnNewOffer += AppService_OnNewOffer;
            AppService.Instance.OnNewChatEvent += AppService_OnNewChatEvent;
            AppService.Instance.OnNewPlayerJoined += AppService_OnNewPlayerJoined;
            AppService.Instance.OnOfferScam += Instance_OnOfferScam;
            AppService.Instance.OnNewTradeChatLine += AppService_OnNewTradeChatLine;
            AppService.Instance.OnNewChaosRecipeResult += AppService_OnNewChaosRecipeResult;
            AppService.Instance.OnToggleChaosRecipeOverlayVisibility += AppService_OnToggleChaosRecipeOverlayVisibility;
            AppService.Instance.ShowTranslateInputControl += AppService_OnShowTranslateInputControl;

            UpdateService.NewUpdateInstalled += UpdateServiceOnNewUpdateInstalled;
        }

        private void AppService_OnShowTranslateInputControl()
        {
            ShowTranslateInputControl();
        }

        public void ShowTranslateInputControl()
        {
            TranslateInputControlVisible = Visibility.Visible;
            OnPropertyChanged("TranslateInputControlVisible");
        }

        public void HideTranslateInputControl()
        {
            TranslateInputControlVisible = Visibility.Hidden;
            OnPropertyChanged("TranslateInputControlVisible");
        }

        public void ShowNewUpdateInstalledMessage()
        {
            AppVersion = $"New update installed! Please restart the application. {AppVersion}";
            OnPropertyChanged("AppVersion");
        }

        public static void SendTranslatedMessage(string message, string targetLanguage = "",string sourceLanguage = "", bool notWhisper = false)
        {
            AppService.Instance.TranslateMessage(message, targetLanguage, sourceLanguage, notWhisper);
        }

        private static string GetAppVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi =
                System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";
            AppService.SetAppVersion(fvi.FileMajorPart, fvi.FileMinorPart, fvi.FileBuildPart);
            return version;
        }

        public static void CheckUpdates()
        {
            UpdateService.CheckUpdates();
        }

        private static void UpdateServiceOnNewUpdateInstalled()
        {
            NotificationService.Instance.ShowNewUpdateInstalledNotification();
        }

        private void AppService_OnToggleChaosRecipeOverlayVisibility(bool show)
        {
            ChaosRecipeOverlayVisible = show ? Visibility.Visible : Visibility.Hidden;
        }

        private void AppService_OnNewChaosRecipeResult(CoreModels.PoeApi.Stash.ChaosRecipeResult result)
        {
            ChaosRecipe = result;
        }

        public void Notify(string name)
        {
            OnPropertyChanged(name);
        }

        private static void AppService_OnNewTradeChatLine(CoreModels.TradeChatLine line)
        {
            NotificationService.Instance.ShowTradeChatMatchNotification(line);
        }

        private void Instance_OnOfferScam(CoreModels.PriceCheckResult result, CoreModels.Offer offer)
        {
            var nbTry = 0;

            while (++nbTry <= 5)
            {
                foreach (var o in Offers)
                {
                    if (o.Id != offer.Id) continue;
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        o.PriceCheck = result;
                        o.PossibleScam = true;
                        UpdateOffers();
                    });
                    return;
                }

                Thread.Sleep(200);
            }
        }

        public void ShowConfigWindow()
        {
            Log.Trace("Showing config window");
            _configWin = new ConfigWindow();
            _configWin.Closed += ConfigWin_Closed;
            _configWin.Show();
        }

        private void ConfigWin_Closed(object sender, EventArgs e)
        {
            Log.Trace("Deleting config window");
            _configWin.Closed -= ConfigWin_Closed;
            _configWin = null;
        }

        private void AppService_OnNewPlayerJoined(string playerName)
        {
            Log.Trace("New player joined event");

            AudioService.Instance.PlayKnock();

            Application.Current.Dispatcher.Invoke(delegate
            {
                foreach (var offer in Offers)
                {
                    if (offer.PlayerName != playerName) continue;
                    Log.Trace($"player \"{playerName}\" joined");
                    offer.PlayerJoined = true;
                }

                UpdateOffers();
            });
        }

        private void AppService_OnNewChatEvent(ChatEventEnum type)
        {
            Log.Trace($"New chat event: {type.ToString()}");

            Application.Current.Dispatcher.Invoke(delegate
            {
                switch (type)
                {
                    case ChatEventEnum.TradeAccepted:
                        var offer = GetActiveOffer();

                        if (offer == null)
                        {
                            return;
                        }

                        offer.State = OfferState.Done;

                        if (offer.IsOutgoing)
                        {
                            SendLeave(offer.Id, true);
                        }
                        else
                        {
                            AppService.Instance.OfferCompleted(new CoreModels.Offer()
                            {
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
                        foreach (var o in Offers)
                        {
                            if (o.TradeRequestSent)
                            {
                                o.State = OfferState.PlayerInvited;
                            }
                        }

                        UpdateOffers();
                        break;
                    case ChatEventEnum.PlayerJoined:
                        break;
                    case ChatEventEnum.PlayerLeft:
                        break;
                    case ChatEventEnum.Offer:
                        break;
                    case ChatEventEnum.AreaJoined:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            });
        }

        public static void SetOverlayHandle(IntPtr handle)
        {
            AppService.Instance.SetOverlayHandle(handle);
        }

        public void UpdateElapsedTime()
        {
            UpdateOffers();
            OnPropertyChanged("Tooltip");
        }

        private void AppService_OnNewOffer(Core.Models.Offer offer)
        {
            Log.Trace("New offer event");
            var config = Config;

            if (config.OnlyShowOffersOfCurrentLeague && config.CurrentLeague != offer.League)
            {
                return;
            }

            if (!offer.IsOutgoing)
            {
                AudioService.Instance.PlayNotification1();
            }

            Application.Current.Dispatcher.Invoke(delegate
            {
                if (!offer.IsOutgoing)
                {
                    if (Offers.Count >= 8)
                    {
                        _overflowOffers.Enqueue(new Offer(offer));
                    }
                    else
                    {
                        Offers.Add(new Offer(offer));
                    }
                }
                else
                {
                    if (OutgoingOffers.Count >= 8)
                    {
                        var buffer = OutgoingOffers.ToList();
                        _overflowOutgoingOffers.Enqueue(buffer.Last());
                        buffer.RemoveAt(buffer.Count - 1);
                        OutgoingOffers.Clear();
                        buffer.ForEach(o => OutgoingOffers.Add(o));
                        OutgoingOffers.Add(new Offer(offer));
                        ReorderOutgoingOffers();
                    }
                    else
                    {
                        OutgoingOffers.Add(new Offer(offer));
                        ReorderOutgoingOffers();
                    }
                }

                OnPropertyChanged("IsOffersFilterVisible");
                OnPropertyChanged("IsOutgoingOffersFilterVisible");
            });
        }

        private void ReorderOutgoingOffers()
        {
            var buffer = OutgoingOffers.ToList()
                .OrderByDescending(o => o.Time);

            OutgoingOffers.Clear();

            foreach (var o in buffer)
            {
                OutgoingOffers.Add(o);
            }
        }

        public List<string> GetLeagues()
        {
            Log.Trace("Getting leagues");
            return AppService.Instance.GetLeagues().Result;
        }

        public Offer GetOffer(int id)
        {
            Log.Trace($"Getting offer {id}");
            var offer = Offers.FirstOrDefault(e => e.Id == id);

            return offer ?? OutgoingOffers.FirstOrDefault(e => e.Id == id);
        }

        private Offer GetActiveOffer()
        {
            Log.Trace("Getting active offer");
            var offer = Offers.FirstOrDefault(o => o.TradeRequestSent);

            return offer ?? OutgoingOffers.FirstOrDefault(o => o.TradeRequestSent);
        }

        private int GetOfferIndex(int id)
        {
            Log.Trace($"Getting offer's index {id}");
            var index = Offers.Select(g => g.Id)
                .ToList()
                .IndexOf(id);

            return index == -1
                ? OutgoingOffers.Select(g => g.Id)
                    .ToList()
                    .IndexOf(id)
                : index;
        }

        private void UpdateOffers()
        {
            Log.Trace("Updating offers");
            var buffer = new Offer[Offers.Count];
            Offers.CopyTo(buffer, 0);
            Offers.Clear();

            foreach (var o in buffer)
            {
                Offers.Add(o);
            }

            var buffer2 = new Offer[OutgoingOffers.Count];
            OutgoingOffers.CopyTo(buffer2, 0);
            OutgoingOffers.Clear();

            foreach (var o in buffer2)
            {
                OutgoingOffers.Add(o);
            }

            OnPropertyChanged("IsOffersFilterVisible");
            OnPropertyChanged("IsOutgoingOffersFilterVisible");
        }

        public void SendTradeRequest(int id, bool isOutgoing = false)
        {
            Log.Trace($"Sending trade request {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            if (isOutgoing)
            {
                if (OutgoingOffers[index].State != OfferState.HideoutJoined)
                {
                    return;
                }

                OutgoingOffers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                AppService.SendTradeChatCommand(OutgoingOffers[index].PlayerName);
            }
            else
            {
                if (!Offers[index].PlayerInvited)
                {
                    return;
                }

                Offers[index].State = OfferState.TradeRequestSent;
                UpdateOffers();

                AppService.SendTradeChatCommand(Offers[index].PlayerName);
            }
        }

        public void SendJoinHideoutCommand(int id)
        {
            Log.Trace($"Sending join hideout command {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            OutgoingOffers[index].State = OfferState.HideoutJoined;
            UpdateOffers();

            AppService.SendHideoutChatCommand(OutgoingOffers[index].PlayerName);
        }

        public void SendBusyWhisper(int id)
        {
            Log.Trace($"Sending busy whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            if (Offers[index].State != OfferState.Initial)
            {
                return;
            }

            AppService.SendChatMessage(
                $"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().BusyWhisper, new CoreModels.Offer() {ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League})}");
        }

        public void SendReInvite(int id)
        {
            Log.Trace($"Sending re-invite commands {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            if (!Offers[index].PlayerInvited)
            {
                return;
            }

            var t = new Thread(delegate()
            {
                AppService.SendKickChatCommand(Offers[index].PlayerName);
                Thread.Sleep(100);
                AppService.SendInviteChatCommand(Offers[index].PlayerName);
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendInvite(int id)
        {
            Log.Trace($"Sending invite command {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            if (Offers[index].State != OfferState.Initial)
            {
                return;
            }

            Offers[index].State = OfferState.PlayerInvited;
            UpdateOffers();

            AppService.SendInviteChatCommand(Offers[index].PlayerName);
        }

        public void SendKick(int id, bool sayThanks = false)
        {
            Log.Trace($"Sending kick command {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            if (Offers[index].State == OfferState.Initial)
            {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            var playerName = Offers[index].PlayerName;

            var t = new Thread(delegate()
            {
                AppService.SendKickChatCommand(playerName);

                if (sayThanks)
                {
                    Thread.Sleep(250);
                    AppService.SendChatMessage(
                        $"@{playerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().ThanksWhisper, new CoreModels.Offer() {ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League})}");
                }

                RemoveOffer(id);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendLeave(int id, bool sayThanks = false)
        {
            Log.Trace($"Sending leave command {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            OutgoingOffers[index].State = OfferState.Done;
            UpdateOffers();

            var playerName = OutgoingOffers[index].PlayerName;

            var t = new Thread(delegate()
            {
                var config = AppService.Instance.GetConfig();

                //if (!string.IsNullOrEmpty(config.PlayerName))
                //{
                //   AppService.SendKickChatCommand(config.PlayerName);
                //}

                if (sayThanks)
                {
                    Thread.Sleep(100);
                    AppService.SendChatMessage(
                        $"@{playerName} {(AppService.Instance.ReplaceVars(config.ThanksWhisper, new CoreModels.Offer() {ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League}))}");
                }

                RemoveOffer(id, true);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void RemoveOffer(int id, bool isOutgoing = false)
        {
            Log.Trace($"Removing offer {id}");
            var index = isOutgoing
                ? OutgoingOffers.Select(e => e.Id)
                    .ToList()
                    .IndexOf(id)
                : Offers.Select(e => e.Id)
                    .ToList()
                    .IndexOf(id);

            if (index != -1)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var refOffers = (isOutgoing ? OutgoingOffers : Offers);
                    refOffers.RemoveAt(index);

                    if (refOffers.Count < 8)
                    {
                        if (isOutgoing)
                        {
                            if (_overflowOutgoingOffers.Count > 0)
                            {
                                OutgoingOffers.Add(_overflowOutgoingOffers.Dequeue());
                                ReorderOutgoingOffers();
                            }
                        }
                        else
                        {
                            if (_overflowOffers.Count > 0)
                            {
                                Offers.Add(_overflowOffers.Dequeue());
                            }
                        }
                    }

                    UpdateOffers();
                    AppService.Instance.FocusGame();
                });
            }
        }

        public void SendStillInterestedWhisper(int id)
        {
            Log.Trace($"Sending still interested whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            UpdateOffers();

            AppService.SendChatMessage(
                $"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().StillInterestedWhisper, new CoreModels.Offer() {ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League})}");
        }

        public void SendSoldWhisper(int id)
        {
            Log.Trace($"Sending sold whisper {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            Offers[index].State = OfferState.Done;
            UpdateOffers();

            AppService.SendChatMessage(
                $"@{Offers[index].PlayerName} {AppService.Instance.ReplaceVars(AppService.Instance.GetConfig().SoldWhisper, new CoreModels.Offer() {ItemName = Offers[index].ItemName, PlayerName = Offers[index].PlayerName, Price = Offers[index].Price, Currency = Offers[index].Currency, League = Offers[index].League})}");

            var offer = Offers[index];
            AppService.Instance.OfferCompleted(new CoreModels.Offer()
            {
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

        public void ClearOffers()
        {
            Log.Trace("Clearing offers");
            AppService.Instance.FocusGame();
            Offers.Clear();


            while (_overflowOffers.Count > 0)
            {
                Offers.Add(_overflowOffers.Dequeue());
            }

            OnPropertyChanged("IsOffersFilterVisible");
        }

        public void ClearOutgoingOffers()
        {
            Log.Trace("Clearing outgoing offers");
            AppService.Instance.FocusGame();
            OutgoingOffers.Clear();

            while (_overflowOutgoingOffers.Count > 0)
            {
                OutgoingOffers.Add(_overflowOutgoingOffers.Dequeue());
            }

            ReorderOutgoingOffers();

            OnPropertyChanged("IsOutgoingOffersFilterVisible");
        }

        public void HighlightItem(int id)
        {
            Log.Trace($"Highlighting offer {id}");
            var index = GetOfferIndex(id);

            if (index == -1)
            {
                return;
            }

            Offers[index].IsHighlighted = true;

            AppService.HighlightStash(Offers[index].EscapedName);
        }

        public void ResetFilter(bool applyToOutgoing = true)
        {
            Log.Trace($"Resetting filter {(applyToOutgoing ? "Outgoing" : "Incoming")} offers");
            if (applyToOutgoing)
            {
                if (_outgoingOffers == null) return;
                OutgoingOffers.Clear();

                foreach (var offer in _outgoingOffers)
                {
                    OutgoingOffers.Add(offer);
                }
            }
            else
            {
                if (_offers == null) return;
                Offers.Clear();

                foreach (var offer in _offers)
                {
                    Offers.Add(offer);
                }
            }
        }

        public void FilterOffers(string searchText, bool applyToOutgoing = true)
        {
            Log.Trace($"Filtering {(applyToOutgoing ? "Outgoing" : "Incoming")} offers with {searchText}");

            searchText = searchText.ToLower().Trim();

            ResetFilter(applyToOutgoing);

            if (applyToOutgoing)
            {
                var results = OutgoingOffers.ToList().FindAll(e =>
                    e.ItemName.ToLower().IndexOf(searchText, StringComparison.Ordinal) != -1 ||
                    e.PlayerName.ToLower().IndexOf(searchText, StringComparison.Ordinal) != -1);

                _outgoingOffers ??= new Offer[OutgoingOffers.Count];

                OutgoingOffers.CopyTo(_outgoingOffers, 0);
                OutgoingOffers.Clear();

                foreach (var r in results)
                {
                    OutgoingOffers.Add(r);
                }
            }
            else
            {
                var results = Offers.ToList().FindAll(e =>
                    e.ItemName.ToLower().IndexOf(searchText, StringComparison.Ordinal) != -1 ||
                    e.PlayerName.ToLower().IndexOf(searchText, StringComparison.Ordinal) != -1);

                _offers ??= new Offer[Offers.Count];

                Offers.CopyTo(_offers, 0);
                Offers.Clear();

                foreach (var r in results)
                {
                    Offers.Add(r);
                }
            }
        }

        public void SetCurrentLeague(string league)
        {
            Log.Trace($"Setting current to {league}");
            var config = Config;
            config.CurrentLeague = league;
            AppService.Instance.SetConfig(new Core.Models.Config()
            {
                Id = config.Id,
                CurrentLeague = config.CurrentLeague,
                OnlyShowOffersOfCurrentLeague = config.OnlyShowOffersOfCurrentLeague,
                PlayerName = config.PlayerName
            });
        }

        public string GetCurrentLeague()
        {
            Log.Trace("Getting current league");
            return Config == null ? "" : Config.CurrentLeague;
        }

        public void ToggleMovableOverlay(TranslateTransform grdOffers, TranslateTransform grdOffersControls,
            TranslateTransform grdOutgoingOffers, TranslateTransform grdChaosRecipe, bool chaosRecipeDockMode = true)
        {
            _isOverlayMovable = !_isOverlayMovable;
            OnPropertyChanged("IncomingOffersGridColor");
            OnPropertyChanged("IncomingOffersControlsGridColor");
            OnPropertyChanged("OutgoingOffersGridColor");

            if (_isOverlayMovable) return;
            var config = Config;
            config.IncomingOffersGridOffset = new System.Drawing.Point((int) grdOffers.X, (int) grdOffers.Y);
            config.IncomingOffersControlsGridOffset =
                new System.Drawing.Point((int) grdOffersControls.X, (int) grdOffersControls.Y);
            config.OutgoingOffersGridOffset =
                new System.Drawing.Point((int) grdOutgoingOffers.X, (int) grdOutgoingOffers.Y);
            config.ChaosRecipeGridOffset = new System.Drawing.Point((int) grdChaosRecipe.X, (int) grdChaosRecipe.Y);
            config.ChaosRecipeOveralyDockMode = chaosRecipeDockMode;
            AppService.Instance.SetConfig(config);
        }
    }
}