using Desktop.Robot;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using Menagerie.Core.Models.PoeApi.Stash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Winook;

namespace Menagerie.Core.Services
{
    public class AppService : IService
    {
        #region Singleton

        private static object _lockInstance = new object();
        private static AppService _instance;

        public static AppService Instance
        {
            get
            {
                lock (_lockInstance)
                {
                    if (_instance == null)
                    {
                        _instance = new AppService();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Events

        public delegate void NewOfferEvent(Offer offer);

        public event NewOfferEvent OnNewOffer;

        public delegate void NewChatEventEvent(Enums.ChatEventEnum evt);

        public event NewChatEventEvent OnNewChatEvent;

        public delegate void NewPlayerJoinedEvent(string playerName);

        public event NewPlayerJoinedEvent OnNewPlayerJoined;

        public delegate void ToggleOverlayVisibilityEvent(bool show);

        public event ToggleOverlayVisibilityEvent OnToggleOverlayVisibility;

        public delegate void OfferScamEvent(PriceCheckResult result, Offer offer);

        public event OfferScamEvent OnOfferScam;

        public delegate void NewTradeChatLineEvent(TradeChatLine line);

        public event NewTradeChatLineEvent OnNewTradeChatLine;

        public delegate void NewChaosRecipeResultEvent(ChaosRecipeResult result);

        public event NewChaosRecipeResultEvent OnNewChaosRecipeResult;

        public delegate void ToggleChaosRecipeOverlayVisibilityEvent(bool show);

        public event ToggleChaosRecipeOverlayVisibilityEvent OnToggleChaosRecipeOverlayVisibility;

        public delegate void ResetDefaultOverlayEvent();

        public event ResetDefaultOverlayEvent OnResetDefaultOverlay;

        #endregion

        private IntPtr _overlayHandle;
        private AppDataService _appDataService;
        private ChatService _chatService;
        private ClientFileService _clientFileService;
        private ClipboardService _clipboardService;
        private CurrencyService _currencyService;
        private GameService _gameService;
        private PoeApiService _poeApiService;
        private PoeWindowService _poeWindowService;
        private KeyboardService _keyboardService;
        private ShortcutService _shortcutService;
        private TradeService _tradeService;
        private PoeNinjaService _poeNinjaService;
        private PriceCheckingService _priceCheckingService;

        private Area _currentArea;
        private static AppVersion _appVersion = new AppVersion();

        private AppService()
        {
            _appDataService = new AppDataService();
            _chatService = new ChatService();
            _clientFileService = new ClientFileService();
            _clipboardService = new ClipboardService();
            _currencyService = new CurrencyService();
            _gameService = new GameService();
            _poeWindowService = new PoeWindowService();
            _poeApiService = new PoeApiService();
            _keyboardService = new KeyboardService();
            _shortcutService = new ShortcutService();
            _tradeService = new TradeService();
            _poeNinjaService = new PoeNinjaService();
            _priceCheckingService = new PriceCheckingService();
        }

        private void SetShortcuts()
        {
            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)116, // F5
                Alt = false,
                Control = false,
                Shift = false,
                Action = Shortcut_GoToHideout
            });
        }

        private void Shortcut_GoToHideout()
        {
            SendHideoutChatCommand();
        }

        public void ResetDefaultOverlay()
        {
            OnResetDefaultOverlay?.Invoke();
        }

        public static void SetAppVersion(int major, int minor, int build)
        {
            _appVersion = new AppVersion(major, minor, build);
        }

        public static AppVersion GetAppVersion()
        {
            return _appVersion;
        }

        public void SetCurrentArea(string area, string type)
        {
            _currentArea = new Area()
            {
                Name = area,
                Type = type
            };
        }

        public Area GetCurrentArea()
        {
            return _currentArea;
        }

        public void SetOverlayHandle(IntPtr handle)
        {
            _overlayHandle = handle;
        }

        public bool EnsurePoeAlive()
        {
            return _poeWindowService.EnsurePoeWindowAlive();
        }

        public IntPtr GetOverlayHandle()
        {
            return _overlayHandle;
        }

        public bool FocusGame()
        {
            return _poeWindowService.Focus();
        }

        public bool GameFocused()
        {
            return _poeWindowService.Focused;
        }

        public void HideOverlay()
        {
            OnToggleOverlayVisibility?.Invoke(true);
        }

        public void ShowOverlay()
        {
            OnToggleOverlayVisibility?.Invoke(false);
        }

        public void ShowChaosRecipeOverlay()
        {
            OnToggleChaosRecipeOverlayVisibility?.Invoke(true);
        }

        public void HideChaosRecipeOverlay()
        {
            OnToggleChaosRecipeOverlayVisibility?.Invoke(false);
        }

        public void ClientFileReady()
        {
            _clientFileService.StartWatching(GetClientFilePath());
        }

        public void EnsureNotHighlightingItem()
        {
            var text = _clipboardService.GetClipboard();

            Thread.Sleep(100);
            SendCtrlA();
            SendCtrlC();

            var newText = _clipboardService.GetClipboard();

            if (text == newText)
            {
                return;
            }

            if (newText.IndexOf("--", StringComparison.Ordinal) != -1 ||
                newText.IndexOf("\n", StringComparison.Ordinal) != -1)
            {
                SendEscape();
            }
        }

        public void NewTradeChatLine(TradeChatLine line)
        {
            OnNewTradeChatLine?.Invoke(line);
        }

        public void PoeWindowReady()
        {
            _keyboardService.HookProcess(_poeWindowService.ProcessId);
            SetShortcuts();
        }

        public void HandleKeyboardInput(KeyboardMessageEventArgs evt)
        {
            _shortcutService.HandleShortcut(evt);
        }

        // TODO: handle chat logEntries for chat scan
        public void NewClientFileLine(string line)
        {
            Task.Run(() =>
            {
                //    _parsingService.ParseClientLine(line);

                if (line.IndexOf("$") == -1)
                {
                    return;
                }

                string loweredLine = line.ToLower().Substring(line.IndexOf("]") + 1);

                var chatScanWords = GetConfig().ChatScanWords;

                foreach (var word in chatScanWords)
                {
                    if (loweredLine.IndexOf(word) != -1)
                    {
                        //   _parsingService.ParseTradeChatLine(line, chatScanWords);
                        break;
                    }
                }
            });
        }

        public string GetClientFilePath()
        {
            return _poeWindowService.ClientFilePath;
        }

        public void NewClipboardText(string text)
        {
            Task.Run(() => { _clientFileService.LogService.Parse(text); });
        }

        public void StashApiUpdated()
        {
            _poeApiService.StashApiReady();
        }

        public void NewChaosRecipeResult(ChaosRecipeResult result)
        {
            OnNewChaosRecipeResult?.Invoke(result);
        }

        public int GetLastOfferId()
        {
            try
            {
                return _appDataService.GetDocuments<Offer>(AppDataService.COLLECTION_TRADES, t => true)
                    .Select(t => t.Id)
                    .Max();
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        public TradeRequest CreateTradeRequest(Offer offer)
        {
            return _poeApiService.CreateTradeRequest(offer);
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request, string league)
        {
            return await _poeApiService.GetTradeRequestResults(request, league);
        }

        public PriceCheckResult GetTradeResults(SearchResult search, int nbResults = 10)
        {
            return _poeApiService.GetTradeResults(search, nbResults);
        }

        public async Task<PriceCheckResult> PriceCheck(Offer offer, int nbResults = 10)
        {
            return await _priceCheckingService.PriceCheck(offer, nbResults);
        }

        public double GetChaosValueOfCurrency(string currency)
        {
            return _poeNinjaService.GetCurrencyChaosValue(currency);
        }

        public double GetChaosValueOfRealNameCurrency(string currency)
        {
            return _currencyService.GetChaosValue(currency);
        }

        public string GetCurrencyRealName(string currency)
        {
            return _currencyService.GetRealName(currency);
        }

        public void SavePoeNinjaCaches(PoeNinjaCaches caches)
        {
            _appDataService.DeleteAllDocument(AppDataService.COLLECTION_POE_NINJA_CACHES);
            _appDataService.InsertDocument<PoeNinjaCaches>(AppDataService.COLLECTION_POE_NINJA_CACHES, caches);
        }

        public PoeNinjaCaches GetPoeNinjaCaches()
        {
            return _appDataService.GetDocument<PoeNinjaCaches>(AppDataService.COLLECTION_POE_NINJA_CACHES);
        }

        public bool IsPoeNinjaCacheReady()
        {
            return _poeNinjaService.CacheReady;
        }

        public string GetCurrencyImageLink(string currencyName)
        {
            return _currencyService.GetCurrencyImageLink(currencyName);
        }

        public void SaveImage(AppImage image)
        {
            _appDataService.InsertDocument(AppDataService.COLLECTION_IMAGES, image);
        }

        public AppImage GetImage(string link)
        {
            return _appDataService.GetDocument<AppImage>(AppDataService.COLLECTION_IMAGES, e => e.Link == link);
        }

        public void NewOffer(Offer offer)
        {
            var config = GetConfig();

            if (config.FilterSoldOffers && _tradeService.IsAlreadySold(offer))
            {
                return;
            }

            if (config.OnlyShowOffersOfCurrentLeague && !offer.IsOutgoing && offer.League != GetConfig().CurrentLeague)
            {
                return;
            }

            if (!string.IsNullOrEmpty(config.PlayerName) && !offer.IsOutgoing)
            {
                Task.Run(() =>
                {
                    try
                    {
                        var priceCheck = _poeApiService.VerifyScam(offer);

                        if (priceCheck != null)
                        {
                            OnOfferScam(priceCheck, offer);
                        }
                    }
                    catch (Exception e)
                    {
                    }
                });
            }

            OnNewOffer(offer);
        }

        public void OfferCompleted(Offer offer)
        {
            _tradeService.AddSoldOffer(offer);
        }

        public List<Offer> GetCompletedTrades()
        {
            return _appDataService.GetDocuments<Offer>(AppDataService.COLLECTION_TRADES)
                .OrderBy(t => t.Time)
                .ToList();
        }

        public void NewPlayerJoined(string playerName)
        {
            OnNewPlayerJoined?.Invoke(playerName);
        }

        public void NewChatEvent(Enums.ChatEventEnum evt)
        {
            OnNewChatEvent?.Invoke(evt);
        }

        public Config GetConfig()
        {
            return _appDataService.GetDocument<Config>(AppDataService.COLLECTION_CONFIG);
        }

        public void SetConfig(Config config)
        {
            var currentConfig = GetConfig();

            _appDataService.UpdateDocument<Config>(AppDataService.COLLECTION_CONFIG, config);

            if (currentConfig == null || currentConfig.CurrentLeague != config.CurrentLeague)
            {
                _poeApiService.UpdateCacheItemsCache();
            }

            if (config.ChaosRecipeEnabled)
            {
                ShowChaosRecipeOverlay();
            }
            else
            {
                HideChaosRecipeOverlay();
            }
        }

        public void SaveTrade(Offer offer)
        {
            _appDataService.InsertDocument<Offer>(AppDataService.COLLECTION_TRADES, offer);
        }

        public async Task<List<string>> GetLeagues()
        {
            return await _poeApiService.GetLeagues();
        }

        public void KeyPress(Key key)
        {
            _keyboardService.KeyPress(key);
        }

        public void KeyUp(Key key)
        {
            _keyboardService.KeyUp(key);
        }

        public void KeyDown(Key key)
        {
            _keyboardService.KeyDown(key);
        }

        public void ClearSpecialKeys()
        {
            _keyboardService.ClearSpecialKeys();
        }

        public void ModifiedKeyStroke(Key modifier, Key key)
        {
            _keyboardService.ModifiedKeyStroke(modifier, key);
        }

        public void SendEnter()
        {
            _keyboardService.SendEnter();
        }

        public void SendCtrlA()
        {
            ModifiedKeyStroke(Key.Control, Key.A);
        }

        public void SendCtrlC()
        {
            ModifiedKeyStroke(Key.Control, Key.C);
        }

        public void SendBackspace()
        {
            _keyboardService.SendBackspace();
        }

        public void SendCtrlV()
        {
            ModifiedKeyStroke(Key.Control, Key.V);
        }

        public void SendEscape()
        {
            _keyboardService.SendEscape();
        }

        public bool SetClipboard(string text)
        {
            return _clipboardService.SetClipboard(text);
        }

        public string ReplaceVars(string msg, Offer offer)
        {
            Area currentArea = GetCurrentArea();

            msg = msg.Replace("{item}", offer.ItemName)
                .Replace("{price}", $"{offer.Price} {offer.Currency}")
                .Replace("{league}", offer.League)
                .Replace("{player}", offer.PlayerName);


            return currentArea != null
                ? msg.Replace("{location}", $"{currentArea.Name} ({currentArea.Type})")
                : msg.Replace("{location}", "Unknown location");
        }

        public void SendTradeChatCommand(string player)
        {
            _chatService.SendTradeCommand(player);
        }

        public void SendHideoutChatCommand(string player)
        {
            _chatService.SendHideoutCommand(player);
        }

        public void SendHideoutChatCommand()
        {
            _chatService.SendHideoutCommand();
        }

        public void SendChatMessage(string msg, int delay = 0)
        {
            _chatService.SendChatMessage(msg, delay);
        }

        public void SendKickChatCommand(string player)
        {
            _chatService.SendKickCommand(player);
        }

        public void SendInviteChatCommand(string player)
        {
            _chatService.SendInviteCommand(player);
        }

        public void HightlightStash(string text)
        {
            _gameService.HightlightStash(text);
        }

        public void Start()
        {
            _appDataService.Start();
            _chatService.Start();
            _clientFileService.Start();
            _clipboardService.Start();
            _currencyService.Start();
            _gameService.Start();
            _keyboardService.Start();
            _poeWindowService.Start();
            _shortcutService.Start();
            _tradeService.Start();
            _poeNinjaService.Start();
            _poeApiService.Start();
            _priceCheckingService.Start();
        }
    }
}