using Menagerie.Core.Abstractions;
using Menagerie.Core.DTOs;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using Winook;

namespace Menagerie.Core.Services {
    public class AppService : IService {
        #region Singleton
        private static object _lock = new object();
        private static AppService _instance = new AppService();
        public static AppService Instance {
            get {
                if (_instance == null) {
                    _instance = new AppService();
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
        #endregion

        private AppDataService _appDataService;
        private ChatService _chatService;
        private ClientFileService _clientFileService;
        private ClipboardService _clipboardService;
        private ConfigService _configService;
        private CurrencyService _currencyService;
        private GameService _gameService;
        private ParsingService _parsingService;
        private PoeApiService _poeApiService;
        private PoeWindowService _poeWindowService;
        private PriceCheckingService _priceCheckingService;
        private PoeNinjaService _poeNinjaService;
        private KeyboardService _keyboardService;
        private ShortcutService _shortcutService;

        private AppService() {
            _appDataService = new AppDataService();
            _chatService = new ChatService();
            _clientFileService = new ClientFileService();
            _clipboardService = new ClipboardService();
            _configService = new ConfigService();
            _currencyService = new CurrencyService();
            _gameService = new GameService();
            _parsingService = new ParsingService();
            _poeApiService = new PoeApiService();
            _poeWindowService = new PoeWindowService();
            _priceCheckingService = new PriceCheckingService();
            _poeApiService = new PoeApiService();
            _poeNinjaService = new PoeNinjaService();
            _keyboardService = new KeyboardService();
            _shortcutService = new ShortcutService();
        }

        private void SetShortcuts() {
            _shortcutService.RegisterShortcut(new Shortcut() {
                Direction = KeyDirection.Any,
                Alt = false,
                Control = true,
                Shift = false,
                Key = VirtualKeyCode.VK_D,
                Action = Shortcut_PriceCheck
            });
        }

        private void Shortcut_PriceCheck() {
            _keyboardService.SendCtrlC();
            string data = _clipboardService.GetClipboard();
            Item item = _parsingService.ParseItem(data);
            PriceCheckResult priceCheck = _priceCheckingService.PriceCheck(item).Result;

            var g = 0;
        }

        public void FocusGame() {
            _poeWindowService.Focus();
        }

        public void ClientFileReady() {
            _clientFileService.StartWatching();
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

        public string GetCurrencyImageLink(string currencyName) {
            return _currencyService.GetCurrencyImageLink(currencyName);
        }

        public List<Tuple<string, BaseType>> GetBaseTypes() {
            return _appDataService.GetBaseTypes();
        }

        public Dictionary<string, MatchStr> GetStatByMatchStr() {
            return _appDataService.GetStatByMatchStr();
        }

        public void NewOffer(Offer offer) {
            OnNewOffer(offer);
        }

        public void NewPlayerJoined(string playerName) {
            OnNewPlayerJoined(playerName);
        }

        public void NewChatEvent(Enums.ChatEventEnum evt) {
            OnNewChatEvent(evt);
        }

        public ConfigDto GetConfig() {
            return _configService.GetConfig();
        }

        public void SetConfig(ConfigDto config) {
            _configService.SetConfig(config);
        }

        public double GetChaosValueOfCurrency(string currency) {
            return _poeNinjaService.GetChaosValue(currency);
        }

        public PriceCheckResult CalculateChaosValues(PriceCheckResult priceCheck) {
            return _currencyService.CalculateChaosValues(priceCheck);
        }

        public TradeRequest CreateTradeRequest(Item item) {
            return _poeApiService.CreateTradeRequest(item);
        }

        public async Task<SearchResult> GetTradeRequestResults(TradeRequest request) {
            return await _poeApiService.GetTradeRequestResults(request);
        }

        public PriceCheckResult GetTradeResults(SearchResult search, Item item, int nbResults = 10) {
            return _poeApiService.GetTradeResults(search, item, nbResults);
        }

        public async Task<PriceCheckResult> PriceCheck(Item item) {
            return await _priceCheckingService.PriceCheck(item);
        }

        public Item ParseItem(string data) {
            return _parsingService.ParseItem(data);
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

        public void SendTradeChatCommand(string player) {
            _chatService.SendTradeCommand(player);
        }

        public void SendHideoutChatCommand(string player) {
            _chatService.SendHideoutCommand(player);
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
            _configService.Start();
            _currencyService.Start();
            _gameService.Start();
            _keyboardService.Start();
            _parsingService.Start();
            _poeApiService.Start();
            _poeNinjaService.Start();
            _poeWindowService.Start();
            _priceCheckingService.Start();
            _shortcutService.Start();
        }
    }
}
