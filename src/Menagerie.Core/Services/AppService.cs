using Menagerie.Core.DTOs;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Menagerie.Core.Services {
    public class AppService : Service {
        #region Singleton
        private static AppService _instance;
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
        public delegate void PoeWindowReadyEvent();
        public event PoeWindowReadyEvent OnPoeWindowReady;

        public delegate void ClienFileReadyEvent();
        public event ClienFileReadyEvent OnClienFileReady;

        public delegate void NewClientFileLineEvent(string line);
        public event NewClientFileLineEvent OnNewClientFileLine;

        public delegate void NewClipboardTextEvent(string text);
        public event NewClipboardTextEvent OnNewClipboardText;

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
        }

        public void FocusGame() {
            _poeWindowService.Focus();
        }

        public void ClientFileReady() {
            OnClienFileReady();
        }

        public void PoeWindowReady() {
            OnPoeWindowReady();
        }

        public void NewClientFileLine(string line) {
            OnNewClientFileLine(line);
            _parsingService.ParseClientLine(line);
        }

        public string GetClientFilePath() {
            return _poeWindowService.ClientFilePath;
        }

        public void NewClipboardText(string text) {
            OnNewClipboardText(text);
            _parsingService.ParseClipboardLine(text);
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

        public bool SetClipboard(string text) {
            return _clipboardService.SetClipboard(text);
        }
    }
}
