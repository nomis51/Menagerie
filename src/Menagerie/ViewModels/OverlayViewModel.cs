﻿using Menagerie.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using Menagerie.Core.Services;
using Menagerie.Core.Enums;
using CoreModels = Menagerie.Core.Models;
using Menagerie.Core.Extensions;
using Menagerie.Services;
using System.Reflection;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Media;
using Caliburn.Micro;
using LiteDB;
using Menagerie.Models.Abstractions;
using Menagerie.Views;
using Serilog;
using MapModifier = Menagerie.Models.MapModifier;

namespace Menagerie.ViewModels
{
    public class OverlayViewModel : Screen
    {
        #region Props

        public ReactiveProperty<Visibility> StackChaosRecipeOverlayVisibility { get; set; }
        public ReactiveProperty<Visibility> DockChaosRecipeOverlayVisibility { get; set; }
        public ReactiveProperty<Visibility> ChaosRecipeOverlayVisibility { get; set; }
        public ReactiveProperty<Visibility> MapModifiersPopupVisibility { get; set; }
        public ReactiveProperty<Visibility> TranslateInputControlVisibility { get; set; }
        public ReactiveProperty<BindableCollection<Offer>> IncomingOffers { get; set; }
        public ReactiveProperty<BindableCollection<Offer>> OutgoingOffers { get; set; }
        public ReactiveProperty<ChaosRecipeResult> ChaosRecipe { get; set; }
        public ReactiveProperty<bool> OverlayMovable { get; set; }
        public ReactiveProperty<BindableCollection<MapModifier>> MapModifiers { get; set; }

        public BindableCollection<string> TargetLanguages { get; set; }

        public BindableCollection<string> SourceLanguages
        {
            get
            {
                if (TargetLanguages == null) return new BindableCollection<string>();

                var langs = new string[TargetLanguages.Count + 1];
                TargetLanguages.CopyTo(langs, 1);
                langs[0] = "Auto";
                return new BindableCollection<string>(langs);
            }
        }

        public Config Config => AppMapper.Instance.Map<CoreModels.Config, Config>(AppService.Instance.GetConfig());
        public int ChaosRecipeGridWidth => DockChaosRecipeOverlayVisibility.Value == Visibility.Visible ? 530 : 60;
        public int ChaosRecipeGridHeight => DockChaosRecipeOverlayVisibility.Value == Visibility.Visible ? 40 : 380;
        public string AppVersion { get; set; } = $"Version {GetAppVersion()}";

        public string CurrentLeague
        {
            get
            {
                var config = AppService.Instance.GetConfig();
                return $"League: {(config != null ? config.CurrentLeague : "Standard")}";
            }
        }

        public Brush IncomingOffersGridColor => OverlayMovable.Value ? Brushes.Blue : Brushes.Transparent;
        public Brush OutgoingOffersGridColor => OverlayMovable.Value ? Brushes.Green : Brushes.Transparent;
        public Brush IncomingOffersControlsGridColor => OverlayMovable.Value ? Brushes.Red : Brushes.Transparent;
        public int MapModifiersPopupX { get; set; }
        public int MapModifiersPopupY { get; set; }

        public Visibility GlovesVisibility => ChaosRecipe.Value.NeedGloves ? Visibility.Visible : Visibility.Hidden;
        public Visibility BootsVisibility => ChaosRecipe.Value.NeedBoots ? Visibility.Visible : Visibility.Hidden;
        public Visibility HelmetsVisibility => ChaosRecipe.Value.NeedHelmets ? Visibility.Visible : Visibility.Hidden;
        public Visibility BodyArmoursVisibility => ChaosRecipe.Value.NeedBodyArmours ? Visibility.Visible : Visibility.Hidden;
        public Visibility RingsVisibility => ChaosRecipe.Value.NeedRings ? Visibility.Visible : Visibility.Hidden;
        public Visibility AmuletsVisibility => ChaosRecipe.Value.NeedAmulets ? Visibility.Visible : Visibility.Hidden;
        public Visibility BeltsVisibility => ChaosRecipe.Value.NeedBelts ? Visibility.Visible : Visibility.Hidden;
        public Visibility WeaponsVisibility => ChaosRecipe.Value.NeedWeapons ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsIncomingOffersFilterVisibility => IncomingOffers.Value.Count > 1 || _fullIncomingOffers != null ? Visibility.Visible : Visibility.Hidden;
        public Visibility IsOutgoingOffersFilterVisibility => OutgoingOffers.Value.Count > 1 || _fullOutgoingOffers != null ? Visibility.Visible : Visibility.Hidden;

        #endregion

        #region Members

        private ConfigView _configWin;
        private readonly Queue<Offer> _overflowIncomingOffers = new();
        private readonly Queue<Offer> _overflowOutgoingOffers = new();
        private Offer[] _fullIncomingOffers;
        private Offer[] _fullOutgoingOffers;
        private ChaosRecipeResult _chaosRecipe = new();

        #endregion


        public OverlayViewModel()
        {
            Log.Information("Initializing OverlayViewModel");

            StackChaosRecipeOverlayVisibility = new ReactiveProperty<Visibility>("StackChaosRecipeOverlayVisibility", this, Visibility.Hidden)
            {
                AdditionalPropertiesToNotify = new List<string>() {"ChaosRecipeGridHeight", "ChaosRecipeGridWidth"}
            };
            DockChaosRecipeOverlayVisibility = new ReactiveProperty<Visibility>("DockChaosRecipeOverlayVisibility", this, Visibility.Visible)
            {
                AdditionalPropertiesToNotify = new List<string>() {"ChaosRecipeGridHeight", "ChaosRecipeGridWidth"}
            };
            ChaosRecipeOverlayVisibility = new ReactiveProperty<Visibility>("ChaosRecipeOverlayVisibility", this, Visibility.Collapsed)
            {
                CustomGet = delegate(Visibility visibility)
                {
                    var config = AppService.Instance.GetConfig();
                    var state = visibility == Visibility.Collapsed
                        ? (config is {ChaosRecipeEnabled: true} ? Visibility.Visible : Visibility.Hidden)
                        : visibility;
                    return state;
                }
            };
            IncomingOffers = new ReactiveProperty<BindableCollection<Offer>>("IncomingOffers", this, new())
            {
                AdditionalPropertiesToNotify = new List<string>()
                {
                    "IsIncomingOffersFilterVisibility"
                }
            };
            OutgoingOffers = new ReactiveProperty<BindableCollection<Offer>>("OutgoingOffers", this, new())
            {
                AdditionalPropertiesToNotify = new List<string>()
                {
                    "IsOutgoingOffersFilterVisibility"
                }
            };
            ChaosRecipe = new ReactiveProperty<ChaosRecipeResult>("ChaosRecipe", this, new ChaosRecipeResult())
            {
                AdditionalPropertiesToNotify = new List<string>()
                {
                    "GlovesVisibility",
                    "BootsVisibility",
                    "HelmetsVisibility",
                    "BeltsVisibility",
                    "BodyArmoursVisibility",
                    "RingsVisibility",
                    "AmuletsVisibility",
                    "WeaponsVisibility",
                }
            };
            OverlayMovable = new ReactiveProperty<bool>("OverlayMovable", this, false)
            {
                AdditionalPropertiesToNotify = new List<string>()
                {
                    "IncomingOffersGridColor",
                    "OutgoingOffersGridColor",
                    "IncomingOffersControlsGridColor",
                }
            };
            MapModifiers = new ReactiveProperty<BindableCollection<MapModifier>>("MapModifiers", this, new BindableCollection<MapModifier>());
            MapModifiersPopupVisibility = new ReactiveProperty<Visibility>("MapModifiersPopupVisibility", this, Visibility.Hidden)
            {
                AdditionalPropertiesToNotify = new List<string>()
                {
                    "MapModifiersPopupX",
                    "MapModifiersPopupX"
                },
                AdditionalReactivePropertiesToNotify = new List<IReactiveProperty>() {MapModifiers}
            };
            TranslateInputControlVisibility = new ReactiveProperty<Visibility>("TranslateInputControlVisibility", this, Visibility.Hidden);

            TargetLanguages = new BindableCollection<string>(AppService.Instance.GetAvailableTranslationLanguages());
            NotifyOfPropertyChange(() => SourceLanguages);

            AppService.Instance.OnNewOffer += AppService_OnNewOffer;
            AppService.Instance.OnNewChatEvent += AppService_OnNewChatEvent;
            AppService.Instance.OnNewPlayerJoined += AppService_OnNewPlayerJoined;
            AppService.Instance.OnOfferScam += Instance_OnOfferScam;
            AppService.Instance.OnNewTradeChatLine += AppService_OnNewTradeChatLine;
            AppService.Instance.OnNewChaosRecipeResult += AppService_OnNewChaosRecipeResult;
            AppService.Instance.OnToggleChaosRecipeOverlayVisibility += AppService_OnToggleChaosRecipeOverlayVisibility;
            AppService.Instance.ShowTranslateInputControl += AppService_OnShowTranslateInputControl;
            AppService.Instance.MapModifiersVerified += AppService_OnMapModifiersVerified;

            UpdateService.NewUpdateInstalled += UpdateServiceOnNewUpdateInstalled;
        }

        private void AppService_OnMapModifiersVerified(List<Core.Models.ItemsScan.MapModifier> modifiers)
        {
            var localModifiers = AppMapper.Instance.Map<List<CoreModels.ItemsScan.MapModifier>, List<MapModifier>>(modifiers);

            var mapModifiers = new BindableCollection<MapModifier>();
            localModifiers.ForEach(mod => mapModifiers.Add(mod));
            MapModifiers.Value = mapModifiers;

            MapModifiersPopupVisibility.Value = Visibility.Visible;
        }

        private void AppService_OnShowTranslateInputControl()
        {
            ShowTranslateInputControl();
        }

        public void ShowTranslateInputControl()
        {
            TranslateInputControlVisibility.Value = Visibility.Visible;
        }

        public void HideTranslateInputControl()
        {
            TranslateInputControlVisibility.Value = Visibility.Hidden;
            AppService.Instance.FocusGame();
        }

        public void ShowNewUpdateInstalledMessage()
        {
            AppVersion = $"New update installed! Please restart the application. {AppVersion}";
        }

        public static void SendTranslatedMessage(string message, string targetLanguage = "", string sourceLanguage = "",
            bool notWhisper = false)
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
            ChaosRecipeOverlayVisibility.Value = show ? Visibility.Visible : Visibility.Hidden;
        }

        private void AppService_OnNewChaosRecipeResult(CoreModels.PoeApi.Stash.ChaosRecipeResult result)
        {
            ChaosRecipe.Value = AppMapper.Instance.Map<CoreModels.PoeApi.Stash.ChaosRecipeResult, ChaosRecipeResult>(result);
        }

        private static void AppService_OnNewTradeChatLine(CoreModels.TradeChatLine line)
        {
            NotificationService.Instance.ShowTradeChatMatchNotification(line);
        }

        private void Instance_OnOfferScam(CoreModels.PriceCheckResult result, Core.Models.Trades.Offer offer)
        {
            var localOffer = AppMapper.Instance.Map<Core.Models.Trades.Offer, Offer>(offer);
            var localPriceCheckResult = AppMapper.Instance.Map<CoreModels.PriceCheckResult, PriceCheckResult>(result);

            var nbTry = 0;

            while (++nbTry <= 5)
            {
                foreach (var o in IncomingOffers.Value.Where(o => o.Id == localOffer.Id))
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        o.PriceCheck = localPriceCheckResult;
                        o.PossibleScam = true;
                        UpdateOffer(o, true);
                    });
                }

                foreach (var o in _overflowIncomingOffers.Where(o => o.Id == localOffer.Id))
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        o.PriceCheck = localPriceCheckResult;
                        o.PossibleScam = true;
                    });
                }

                Thread.Sleep(200);
            }
        }

        public void ShowConfigWindow()
        {
            Log.Information("Showing config window");
            AppBootstrapper.WindowManager.ShowWindowAsync(new ConfigViewModel());
        }

        private void ConfigWin_Closed(object sender, EventArgs e)
        {
            Log.Information("Deleting config window");
            _configWin.Closed -= ConfigWin_Closed;
            _configWin = null;
        }

        private void AppService_OnNewPlayerJoined(string playerName)
        {
            Log.Information("New player joined event");

            AudioService.Instance.PlayKnock();

            Application.Current.Dispatcher.Invoke(delegate
            {
                foreach (var offer in IncomingOffers.Value.Where(offer => offer.PlayerName == playerName))
                {
                    Log.Information($"player \"{playerName}\" joined");

                    offer.PlayerJoined = true;
                    UpdateOffer(offer, true);
                }

                foreach (var offer in _overflowIncomingOffers.Where(offer => offer.PlayerName == playerName))
                {
                    Log.Information($"player \"{playerName}\" joined");

                    offer.PlayerJoined = true;
                }
            });
        }

        private void AppService_OnNewChatEvent(ChatEventEnum type)
        {
            Log.Information($"New chat event: {type.ToString()}");

            Application.Current.Dispatcher.Invoke(delegate
            {
                switch (type)
                {
                    case ChatEventEnum.TradeAccepted:
                        var offer = GetActiveOffer();

                        if (offer == null) return;

                        offer.State = OfferState.Done;

                        if (offer.IsOutgoing)
                        {
                            SendLeave(offer.Id, true);
                        }
                        else
                        {
                            AppService.Instance.OfferCompleted(AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer));

                            SendKick(offer.Id, AppService.Instance.GetConfig().AutoThanks);
                        }

                        break;

                    case ChatEventEnum.TradeCancelled:
                        foreach (var o in IncomingOffers.Value.Where(o => o.TradeRequestSent))
                        {
                            o.State = OfferState.PlayerInvited;
                            UpdateOffer(o, true);
                        }

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
                        Log.Warning($"Unhandled chat event type {type}");
                        break;
                }
            });
        }

        public static void SetOverlayHandle(IntPtr handle)
        {
            AppService.Instance.SetOverlayHandle(handle);
        }

        private void OrderOffer(ref BindableCollection<Offer> offers, Func<Offer, DateTime> orderByFn, bool isDesc = false)
        {
            var reorderedOffers = new BindableCollection<Offer>();
            var tempOffers = isDesc ? offers.OrderByDescending(orderByFn) : offers.OrderBy(orderByFn);

            foreach (var offer in tempOffers)
            {
                reorderedOffers.Add(offer);
            }

            offers = reorderedOffers;
        }

        private void InsertOffer(Offer offer, bool isOutgoing = false)
        {
            var offers = isOutgoing ? OutgoingOffers : IncomingOffers;
            var overflowOffers = isOutgoing ? _overflowOutgoingOffers : _overflowIncomingOffers;

            if (offers.Value.Count >= 8)
            {
                overflowOffers.Enqueue(offer);
                return;
            }

            if (!isOutgoing)
            {
                offers.Value.Insert(0, offer);
            }
            else
            {
                offers.Value.Add(offer);

                var bufferOffers = offers.Value;
                OrderOffer(ref bufferOffers, o => o.Time, true);
                offers.Value = bufferOffers;
            }

            offers.Notify();
        }

        private void RemoveOffer(Offer offer, bool isOutgoing = false)
        {
            RemoveOffer(offer.Id, isOutgoing);
        }

        public void RemoveOffer(ObjectId id, bool isOutgoing = false)
        {
            var offers = isOutgoing ? OutgoingOffers : IncomingOffers;
            var overflowOffers = isOutgoing ? _overflowOutgoingOffers : _overflowIncomingOffers;

            var index = offers.Value.Select(o => o.Id).ToList().IndexOf(id);

            if (index == -1) return;

            offers.Value.RemoveAt(index);

            if (overflowOffers.Count <= 0 || offers.Value.Count >= 8) return;

            while (offers.Value.Count < 8)
            {
                offers.Value.Add(overflowOffers.Dequeue());
            }

            offers.Notify();
        }

        private void AppService_OnNewOffer(Core.Models.Trades.Offer offer)
        {
            var localOffer = AppMapper.Instance.Map<Core.Models.Trades.Offer, Offer>(offer);

            Log.Information("New offer event");

            if (Config.OnlyShowOffersOfCurrentLeague && Config.CurrentLeague != localOffer.League) return;

            if (!localOffer.IsOutgoing)
            {
                AudioService.Instance.PlayNotification1();
            }

            Application.Current.Dispatcher.Invoke(delegate { InsertOffer(localOffer, localOffer.IsOutgoing); });
        }

        public List<string> GetLeagues()
        {
            Log.Information("Getting leagues");
            return AppService.Instance.GetLeagues().Result;
        }

        public Offer GetOffer(ObjectId id, bool isOutgoing = false)
        {
            Log.Information($"Getting offer {id}");
            return (isOutgoing ? OutgoingOffers : IncomingOffers).Value.FirstOrDefault(o => o.Id == id);
        }

        private Offer GetActiveOffer(bool isOutgoing = false)
        {
            Log.Information("Getting active offer");
            return (isOutgoing ? OutgoingOffers : IncomingOffers).Value.FirstOrDefault(o => o.TradeRequestSent);
        }

        private int GetOfferIndex(string id, bool isOutgoing = false)
        {
            Log.Information($"Getting offer's index {id}");
            return (isOutgoing ? OutgoingOffers : IncomingOffers).Value.Select(o => o.Id).ToList().IndexOf(new ObjectId(id));
        }

        private void UpdateOffer(Offer offer, bool fullUpdate = false)
        {
            if (offer.IsOutgoing)
            {
                if (fullUpdate)
                {
                    var buffer = new Offer[OutgoingOffers.Value.Count];
                    OutgoingOffers.Value.CopyTo(buffer, 0);

                    var temp = new BindableCollection<Offer>();

                    foreach (var o in buffer)
                    {
                        temp.Add(o);
                    }

                    OutgoingOffers.Value = temp;
                }

                OutgoingOffers.Notify();
            }
            else
            {
                if (fullUpdate)
                {
                    var buffer = new Offer[IncomingOffers.Value.Count];
                    IncomingOffers.Value.CopyTo(buffer, 0);

                    var temp = new BindableCollection<Offer>();

                    foreach (var o in buffer)
                    {
                        temp.Add(o);
                    }

                    IncomingOffers.Value = temp;
                }

                IncomingOffers.Notify();
            }
        }

        public void SendTradeRequest(ObjectId id, bool isOutgoing = false)
        {
            Log.Information($"Sending trade request {id}");
            var offer = GetOffer(id, isOutgoing);

            if (offer == null) return;

            switch (isOutgoing)
            {
                case false when !offer.PlayerInvited:
                    return;
                default:
                    offer.State = OfferState.TradeRequestSent;
                    UpdateOffer(offer, true);
                    AppService.SendTradeChatCommand(offer.PlayerName);
                    break;
            }
        }

        public void SendJoinHideoutCommand(ObjectId id)
        {
            Log.Information($"Sending join hideout command {id}");
            var offer = GetOffer(id, true);

            if (offer == null) return;

            offer.State = OfferState.HideoutJoined;
            UpdateOffer(offer);

            AppService.SendHideoutChatCommand(offer.PlayerName);
        }

        public void SendBusyWhisper(ObjectId id)
        {
            Log.Information($"Sending busy whisper {id}");
            var offer = GetOffer(id);

            if (offer is not {State: OfferState.Initial}) return;

            AppService.SendChatMessage(
                $"@{offer.PlayerName} {AppService.Instance.ReplaceVars(Config.BusyWhisper, AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer))}");
        }

        public void SendReInvite(ObjectId id)
        {
            Log.Information($"Sending re-invite commands {id}");
            var offer = GetOffer(id);

            if (offer is not {PlayerInvited: true}) return;

            var t = new Thread(delegate()
            {
                AppService.SendKickChatCommand(offer.PlayerName);
                Thread.Sleep(100);
                AppService.SendInviteChatCommand(offer.PlayerName);
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendInvite(ObjectId id)
        {
            Log.Information($"Sending invite command {id}");
            var offer = GetOffer(id);

            if (offer is not {State: OfferState.Initial}) return;

            offer.State = OfferState.PlayerInvited;
            UpdateOffer(offer, true);

            AppService.SendInviteChatCommand(offer.PlayerName);
        }

        public void SendKick(ObjectId id, bool sayThanks = false)
        {
            Log.Information($"Sending kick command {id}");
            var offer = GetOffer(id);

            if (offer == null) return;

            if (offer.State == OfferState.Initial) return;

            offer.State = OfferState.Done;
            UpdateOffer(offer);

            var t = new Thread(delegate()
            {
                AppService.SendKickChatCommand(offer.PlayerName);

                if (sayThanks)
                {
                    Thread.Sleep(250);
                    AppService.SendChatMessage(
                        $"@{offer.PlayerName} {AppService.Instance.ReplaceVars(Config.ThanksWhisper, AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer))}");
                }

                RemoveOffer(offer);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendLeave(ObjectId id, bool sayThanks = false)
        {
            Log.Information($"Sending leave command {id}");
            var offer = GetOffer(id, true);

            if (offer == null) return;

            offer.State = OfferState.Done;
            UpdateOffer(offer);

            var t = new Thread(delegate()
            {
                if (sayThanks)
                {
                    Thread.Sleep(100);
                    AppService.SendChatMessage(
                        $"@{offer.PlayerName} {(AppService.Instance.ReplaceVars(Config.ThanksWhisper, AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer)))}");
                }

                RemoveOffer(offer, true);
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        public void SendStillInterestedWhisper(ObjectId id)
        {
            Log.Information($"Sending still interested whisper {id}");
            var offer = GetOffer(id);

            if (offer == null) return;

            AppService.SendChatMessage(
                $"@{offer.PlayerName} {AppService.Instance.ReplaceVars(Config.StillInterestedWhisper, AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer))}");
        }

        public void SendSoldWhisper(ObjectId id)
        {
            Log.Information($"Sending sold whisper {id}");
            var offer = GetOffer(id);

            if (offer == null) return;

            offer.State = OfferState.Done;
            UpdateOffer(offer);

            var mappedOffer = AppMapper.Instance.Map<Offer, Core.Models.Trades.Offer>(offer);

            AppService.SendChatMessage(
                $"@{offer.PlayerName} {AppService.Instance.ReplaceVars(Config.SoldWhisper, mappedOffer)}");

            AppService.Instance.OfferCompleted(mappedOffer);

            RemoveOffer(offer);
        }

        public void RemoveAllOffers(bool isOutgoing = false)
        {
            Log.Information("Clearing offers");
            AppService.Instance.FocusGame();

            var offers = isOutgoing ? OutgoingOffers : IncomingOffers;
            offers.Value = new BindableCollection<Offer>();

            var overflowOffers = isOutgoing ? _overflowOutgoingOffers : _overflowIncomingOffers;

            if (overflowOffers.Count == 0) return;


            while (offers.Value.Count < 8)
            {
                offers.Value.Add(overflowOffers.Dequeue());
            }

            if (!isOutgoing) return;

            var bufferOffers = offers.Value;
            OrderOffer(ref bufferOffers, o => o.Time, true);
            offers.Value = bufferOffers;
        }

        public void HighlightItem(ObjectId id)
        {
            Log.Information($"Highlighting offer {id}");
            var offer = GetOffer(id);

            if (offer == null) return;

            offer.IsHighlighted = true;

            AppService.HighlightStash(offer.EscapedName);
        }

        public void ResetFilter(bool applyToOutgoing = false)
        {
            Log.Information($"Resetting filter {(applyToOutgoing ? "Outgoing" : "Incoming")} offers");

            var fullOffers = applyToOutgoing ? _fullOutgoingOffers : _fullIncomingOffers;

            if (fullOffers == null || !fullOffers.Any()) return;

            var offers = new BindableCollection<Offer>();

            foreach (var offer in fullOffers)
            {
                offers.Add(offer);
            }

            if (applyToOutgoing)
            {
                OutgoingOffers.Value = offers;
                _fullOutgoingOffers = null;
            }
            else
            {
                IncomingOffers.Value = offers;
                _fullIncomingOffers = null;
            }
        }

        public void FilterOffers(string searchText, bool applyToOutgoing = false)
        {
            Log.Information($"Filtering {(applyToOutgoing ? "Outgoing" : "Incoming")} offers with {searchText}");

            ResetFilter(applyToOutgoing);

            if (string.IsNullOrEmpty(searchText)) return;

            searchText = searchText.ToLower().Trim();

            var results = (applyToOutgoing ? OutgoingOffers : IncomingOffers).Value.ToList()
                .FindAll(o => o.ItemName.ToLower().Contains(searchText) || o.PlayerName.ToLower().Contains(searchText));

            if (!results.Any()) return;

            Offer[] fullOffers;

            if (applyToOutgoing)
            {
                _fullOutgoingOffers = new Offer[OutgoingOffers.Value.Count];
                fullOffers = _fullOutgoingOffers;
            }
            else
            {
                _fullIncomingOffers = new Offer[IncomingOffers.Value.Count];
                fullOffers = _fullIncomingOffers;
            }

            var offers = applyToOutgoing ? OutgoingOffers : IncomingOffers;
            offers.Value.ToList().CopyTo(fullOffers, 0);

            offers.Value = new BindableCollection<Offer>();
            offers.Value.AddRange(results);
            offers.Notify();
        }

        public void SetCurrentLeague(string league)
        {
            Log.Information($"Setting current to {league}");
            var config = Config;
            config.CurrentLeague = league;
            AppService.Instance.SetConfig(AppMapper.Instance.Map<Config, CoreModels.Config>(config));
        }

        public string GetCurrentLeague()
        {
            Log.Information("Getting current league");
            return Config == null ? "" : Config.CurrentLeague;
        }

        public void ToggleMovableOverlay(TranslateTransform grdOffers, TranslateTransform grdOffersControls,
            TranslateTransform grdOutgoingOffers, TranslateTransform grdChaosRecipe, bool chaosRecipeDockMode = true)
        {
            OverlayMovable.Value = !OverlayMovable.Value;

            if (OverlayMovable.Value) return;

            var config = Config;

            config.IncomingOffersGridOffset = new System.Drawing.Point((int) grdOffers.X, (int) grdOffers.Y);
            config.IncomingOffersControlsGridOffset =
                new System.Drawing.Point((int) grdOffersControls.X, (int) grdOffersControls.Y);
            config.OutgoingOffersGridOffset =
                new System.Drawing.Point((int) grdOutgoingOffers.X, (int) grdOutgoingOffers.Y);
            config.ChaosRecipeGridOffset = new System.Drawing.Point((int) grdChaosRecipe.X, (int) grdChaosRecipe.Y);
            config.ChaosRecipeOveralyDockMode = chaosRecipeDockMode;

            AppService.Instance.SetConfig(AppMapper.Instance.Map<Config, CoreModels.Config>(config));
        }
    }
}