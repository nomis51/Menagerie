using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using Menagerie.Core.Models.PoeApi.Stash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput.Native;
using Winook;

namespace Menagerie.Core.Services {
    public class AppService : IService {
        #region Singleton
        private static object _lockInstance = new object();
        private static AppService _instance;
        public static AppService Instance {
            get {
                lock (_lockInstance) {
                    if (_instance == null) {
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
        #endregion

        private IntPtr _overlayHandle;
        private AppDataService _appDataService;
        private ChatService _chatService;
        private ClientFileService _clientFileService;
        private ClipboardService _clipboardService;
        private CurrencyService _currencyService;
        private GameService _gameService;
        private ParsingService _parsingService;
        private PoeApiService _poeApiService;
        private PoeWindowService _poeWindowService;
        private KeyboardService _keyboardService;
        private ShortcutService _shortcutService;
        private TradeService _tradeService;
        private PoeNinjaService _poeNinjaService;
        private PriceCheckingService _priceCheckingService;

        private AppService() {
            _appDataService = new AppDataService();
            _chatService = new ChatService();
            _clientFileService = new ClientFileService();
            _clipboardService = new ClipboardService();
            _currencyService = new CurrencyService();
            _gameService = new GameService();
            _parsingService = new ParsingService();
            _poeWindowService = new PoeWindowService();
            _poeApiService = new PoeApiService();
            _keyboardService = new KeyboardService();
            _shortcutService = new ShortcutService();
            _tradeService = new TradeService();
            _poeNinjaService = new PoeNinjaService();
            _priceCheckingService = new PriceCheckingService();
        }

        private void SetShortcuts() {
            _shortcutService.RegisterShortcut(new Shortcut() {
                Direction = KeyDirection.Down,
                Key = VirtualKeyCode.F5,
                Alt = false,
                Control = false,
                Shift = false,
                Action = Shortcut_GoToHideout
            });
        }

        private void Shortcut_GoToHideout() {
            SendHideoutChatCommand();
        }

        public void SetOverlayHandle(IntPtr handle) {
            _overlayHandle = handle;
        }

        public bool EnsurePoeAlive() {
            return _poeWindowService.EnsurePoeWindowAlive();
        }

        public IntPtr GetOverlayHandle() {
            return _overlayHandle;
        }

        public void FocusGame() {
            _poeWindowService.Focus();
        }

        public bool GameFocused() {
            return _poeWindowService.Focused;
        }

        public void HideOverlay() {
            OnToggleOverlayVisibility(true);
        }

        public void ShowOverlay() {
            OnToggleOverlayVisibility(false);
        }

        public void ShowChaosRecipeOverlay() {
            OnToggleChaosRecipeOverlayVisibility(true);
        }

        public void HideChaosRecipeOverlay() {
            OnToggleChaosRecipeOverlayVisibility(false);
        }

        public void ClientFileReady() {
            _clientFileService.StartWatching();
        }

        public void EnsureNotHighlightingItem() {
            string text = _clipboardService.GetClipboard();

            Thread.Sleep(100);
            SendCtrlA();
            SendCtrlC();

            string newText = _clipboardService.GetClipboard();

            if (text == newText) {
                return;
            }

            if (newText.IndexOf("--") != -1 || newText.IndexOf("\n") != -1) {
                SendEscape();
            }
        }

        public void NewTradeChatLine(TradeChatLine line) {
            OnNewTradeChatLine(line);
        }

        public void PoeWindowReady() {
            _keyboardService.HookProcess(_poeWindowService.ProcessId);
            SetShortcuts();
        }

        public void HandleKeyboardInput(KeyboardMessageEventArgs evt) {
            _shortcutService.HandleShortcut(evt);
        }

        public void NewClientFileLine(string line) {
            Task.Run(() => {
                _parsingService.ParseClientLine(line);

                var chatScanWords = GetConfig().ChatScanWords;
                string loweredLine = line.ToLower().Substring(line.IndexOf("]") + 1);

                foreach (var word in chatScanWords) {
                    if (loweredLine.IndexOf(word) != -1) {
                        _parsingService.ParseTradeChatLine(line, chatScanWords);
                        break;
                    }
                }
            });
        }

        public string GetClientFilePath() {
            return _poeWindowService.ClientFilePath;
        }

        public void NewClipboardText(string text) {
            Task.Run(() => {
                _parsingService.ParseClipboardLine(text);
            });
        }

        public void StashApiUpdated() {
            _poeApiService.StashApiReady();
        }

        public void NewChaosRecipeResult(ChaosRecipeResult result) {
            OnNewChaosRecipeResult(result);
        }

        public int GetLastOfferId() {
            try {
                return _appDataService.GetDocuments<Offer>(AppDataService.COLLECTION_TRADES, t => true)
                    .Select(t => t.Id)
                    .Max();
            } catch (Exception e) {
                return 0;
            }
        }

        public TradeRequest CreateTradeRequest(Offer offer) {
            return _poeApiService.CreateTradeRequest(offer);
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request, string league) {
            return await _poeApiService.GetTradeRequestResults(request, league);
        }

        public PriceCheckResult GetTradeResults(SearchResult search, int nbResults = 10) {
            return _poeApiService.GetTradeResults(search, nbResults);
        }

        public async Task<PriceCheckResult> PriceCheck(Offer offer, int nbResults = 10) {
            return await _priceCheckingService.PriceCheck(offer, nbResults);
        }

        public double GetChaosValueOfCurrency(string currency) {
            return _poeNinjaService.GetCurrencyChaosValue(currency);
        }

        public double GetChaosValueOfRealNameCurrency(string currency) {
            return _currencyService.GetChaosValue(currency);
        }

        public string GetCurrencyRealName(string currency) {
            return _currencyService.GetRealName(currency);
        }

        public void SavePoeNinjaCaches(PoeNinjaCaches caches) {
            _appDataService.DeleteAllDocument(AppDataService.COLLECTION_POE_NINJA_CACHES);
            _appDataService.InsertDocument<PoeNinjaCaches>(AppDataService.COLLECTION_POE_NINJA_CACHES, caches);
        }

        public PoeNinjaCaches GetPoeNinjaCaches() {
            return _appDataService.GetDocument<PoeNinjaCaches>(AppDataService.COLLECTION_POE_NINJA_CACHES);
        }

        public bool IsPoeNinjaCacheReady() {
            return _poeNinjaService.CacheReady;
        }

        public string GetCurrencyImageLink(string currencyName) {
            return _currencyService.GetCurrencyImageLink(currencyName);
        }

        public void SaveImage(AppImage image) {
            _appDataService.InsertDocument(AppDataService.COLLECTION_IMAGES, image);
        }

        public AppImage GetImage(string link) {
            return _appDataService.GetDocument<AppImage>(AppDataService.COLLECTION_IMAGES, e => e.Link == link);
        }

        public void NewOffer(Offer offer) {
            var config = GetConfig();

            if (config.FilterSoldOffers && _tradeService.IsAlreadySold(offer)) {
                return;
            }

            if (config.OnlyShowOffersOfCurrentLeague && !offer.IsOutgoing && offer.League != GetConfig().CurrentLeague) {
                return;
            }

            if (!string.IsNullOrEmpty(config.PlayerName) && !offer.IsOutgoing) {
                Task.Run(() => {
                    try {
                        var priceCheck = _poeApiService.VerifyScam(offer);

                        if (priceCheck != null) {
                            OnOfferScam(priceCheck, offer);
                        }
                    } catch (Exception e) {

                    }
                });
            }

            OnNewOffer(offer);
        }

        public void OfferCompleted(Offer offer) {
            _tradeService.AddSoldOffer(offer);
        }

        public List<Offer> GetCompletedTrades() {
            return _appDataService.GetDocuments<Offer>(AppDataService.COLLECTION_TRADES)
                .OrderBy(t => t.Time)
                .ToList();
        }

        public void NewPlayerJoined(string playerName) {
            OnNewPlayerJoined(playerName);
        }

        public void NewChatEvent(Enums.ChatEventEnum evt) {
            OnNewChatEvent(evt);
        }

        public Config GetConfig() {
            return _appDataService.GetDocument<Config>(AppDataService.COLLECTION_CONFIG);
        }

        public void SetConfig(Config config) {
            var currentConfig = GetConfig();

            _appDataService.UpdateDocument<Config>(AppDataService.COLLECTION_CONFIG, config);

            if (currentConfig.CurrentLeague != config.CurrentLeague) {
                _poeApiService.UpdateCacheItemsCache();
            }

            if (config.ChaosRecipeEnabled) {
                ShowChaosRecipeOverlay();
            } else {
                HideChaosRecipeOverlay();
            }
        }

        public void SaveTrade(Offer offer) {
            _appDataService.InsertDocument<Offer>(AppDataService.COLLECTION_TRADES, offer);
        }

        public async Task<List<string>> GetLeagues() {
            return await _poeApiService.GetLeagues();
        }

        public void KeyPress(VirtualKeyCode key) {
            _keyboardService.KeyPress(key);
        }

        public void KeyUp(VirtualKeyCode key) {
            _keyboardService.KeyUp(key);
        }

        public void KeyDown(VirtualKeyCode key) {
            _keyboardService.KeyDown(key);
        }

        public void ClearSpecialKeys() {
            _keyboardService.ClearSpecialKeys();
        }

        public void ModifiedKeyStroke(VirtualKeyCode modifier, VirtualKeyCode key) {
            _keyboardService.ModifiedKeyStroke(modifier, key);
        }

        public void SendEnter() {
            _keyboardService.SendEnter();
        }

        public void SendCtrlA() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
        }

        public void SendCtrlC() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
        }

        public void SendBackspace() {
            _keyboardService.SendBackspace();
        }

        public void SendCtrlV() {
            ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        public void SendEscape() {
            _keyboardService.SendEscape();
        }

        public bool SetClipboard(string text) {
            return _clipboardService.SetClipboard(text);
        }

        public string ReplaceVars(string msg, Offer offer) {
            return msg.Replace("{item}", offer.ItemName)
                .Replace("{price}", $"{offer.Price} {offer.Currency}")
                .Replace("{league}", offer.League)
                .Replace("{player}", offer.PlayerName);
        }

        public void SendTradeChatCommand(string player) {
            _chatService.SendTradeCommand(player);
        }

        public void SendHideoutChatCommand(string player) {
            _chatService.SendHideoutCommand(player);
        }

        public void SendHideoutChatCommand() {
            _chatService.SendHideoutCommand();
        }

        public void SendChatMessage(string msg) {
            _chatService.SendChatMessage(msg);
        }

        public void SendKickChatCommand(string player) {
            _chatService.SendKickCommand(player);
        }

        public void SendInviteChatCommand(string player) {
            _chatService.SendInviteCommand(player);
        }

        public void HightlightStash(string text) {
            _gameService.HightlightStash(text);
        }

        public void Start() {
            _appDataService.Start();
            _chatService.Start();
            _clientFileService.Start();
            _clipboardService.Start();
            _currencyService.Start();
            _gameService.Start();
            _keyboardService.Start();
            _parsingService.Start();
            _poeWindowService.Start();
            _shortcutService.Start();
            _tradeService.Start();
            _poeNinjaService.Start();
            _poeApiService.Start();
            _priceCheckingService.Start();
        }
    }
}
