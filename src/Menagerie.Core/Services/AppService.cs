using Desktop.Robot;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using Menagerie.Core.Models.PoeApi.Stash;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Menagerie.Core.Enums;
using Menagerie.Core.Models.ItemsScan;
using Menagerie.Core.Models.ML;
using Menagerie.Core.Models.Trades;
using Menagerie.Core.Models.Translator;
using PoeLogsParser.Enums;
using PoeLogsParser.Models;
using Winook;

namespace Menagerie.Core.Services
{
    public sealed class AppService : IService
    {
        #region Singleton

        private static readonly object LockInstance = new object();
        private static AppService _instance;

        public static AppService Instance
        {
            get
            {
                lock (LockInstance)
                {
                    _instance ??= new AppService();
                }

                return _instance;
            }
        }

        #endregion

        #region Constants

        public readonly Size TradeWindowSquareSize = new(53, 53);
        public readonly Size TradeWindowRowsAndColumns = new(12, 5);
        public readonly Rectangle TradeWindowBounds = new(311, 203, 632, 264);

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

        public delegate void TextTranslatedEvent(ChatMessageTranslation translation);

        public event TextTranslatedEvent OnTextTranslated;

        public delegate void ShowTranslateInputControlEvent();

        public event ShowTranslateInputControlEvent ShowTranslateInputControl;

        public delegate void MapModifiersVerifiedEvent(List<MapModifier> modifiers);

        public event MapModifiersVerifiedEvent MapModifiersVerified;

        public delegate void MouseMovedEvent();

        public event MouseMovedEvent OnMouseMoved;

        public delegate void TradeWindowScannedEvent(float chaosValue, float exaltedValue, List<AiCurrencyAnalysis> aiCurrencyAnalyses, PredictionResponse response);

        public event TradeWindowScannedEvent OnTradeWindowScanned;

        #endregion

        private IntPtr _overlayHandle;
        private readonly AppDataService _appDataService;
        private readonly ChatService _chatService;
        private readonly ClientFileService _clientFileService;
        private readonly ClipboardService _clipboardService;
        private readonly CurrencyService _currencyService;
        private readonly GameService _gameService;
        private readonly PoeApiService _poeApiService;
        private readonly PoeWindowService _poeWindowService;
        private readonly KeyboardService _keyboardService;
        private readonly ShortcutService _shortcutService;
        private readonly TradeService _tradeService;
        private readonly PoeNinjaService _poeNinjaService;
        private readonly PriceCheckingService _priceCheckingService;
        private readonly TranslateService _translateService;
        private readonly ItemService _itemService;
        private readonly AppAiService _appAiService;
        private readonly ScreenCaptureService _screenCaptureService;
        private readonly CloudDataService _cloudDataService;

        private Area _currentArea;
        private static AppVersion _appVersion = new();
        private readonly Dictionary<string, string> _translationTable = new Dictionary<string, string>();
        private bool _runningAiAnalysis;

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
            _translateService = new TranslateService();
            _itemService = new ItemService();
            _appAiService = new AppAiService();
            _screenCaptureService = new ScreenCaptureService();
            _cloudDataService = new CloudDataService();
        }

        private void SetShortcuts()
        {
            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)117, // F6
                Alt = false,
                Control = false,
                Shift = false,
                Action = () => AnalyzeTradeWindow().Wait()
            });

            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)116, // F5
                Alt = false,
                Control = false,
                Shift = false,
                Action = Shortcut_GoToHideout
            });

            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)84, // T
                Alt = false,
                Control = true,
                Shift = true,
                Action = DoShowTranslateInputControl
            });

            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)70, // F
                Alt = false,
                Control = true,
                Shift = false,
                Action = () => SearchItemInStash()
            });

            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)70, // F
                Alt = false,
                Control = true,
                Shift = true,
                Action = () => SearchItemInStash(true)
            });

            _shortcutService.RegisterShortcut(new Shortcut()
            {
                Direction = KeyDirection.Down,
                Key = (Key)70, // F
                Alt = true,
                Control = false,
                Shift = false,
                Action = VerifyMapModifiers
            });
        }

        public async Task AnalyzeTradeWindow()
        {
            if (_runningAiAnalysis) return;

            _runningAiAnalysis = true;

            var image = _screenCaptureService.CaptureArea(TradeWindowBounds);
            var imageIds = _appAiService.ProcessAndSaveTradeWindowImage(image);
            var request = new PredictionRequest();
            imageIds.ForEach(i => request.Images.Add(new PredictionRequestImage
            {
                FileId = i.Item1,
                FilePath = i.Item2,
                Models = new List<string>
                        {
                            TrainedModelTypeConverter.Convert(TrainedModelType.CurrencyType),
                            TrainedModelTypeConverter.Convert(TrainedModelType.StackSize),
                        }
            }));
            var result = await _appAiService.Predict(request);

            List<AiCurrencyAnalysis> aiCurrencyAnalyses = new();
            double chaosValue = 0.0f;
            double exaltedValue = 0.0f;

            foreach (var i in result.Images)
            {
                var currencyTypeValue = i.Predictions.Find(p => p.Model == "currency_type");
                if (string.IsNullOrEmpty(currencyTypeValue.Value)) continue;

                var stackSizeValue = i.Predictions.Find(p => p.Model == "stack_size");
                if (string.IsNullOrEmpty(stackSizeValue.Value)) continue;

                var normalizedCurrencyName = CurrencyService.AiCurrencyToNormzlizedCurrency(currencyTypeValue.Value);
                var realCurrencyName = CurrencyService.GetRealName(normalizedCurrencyName);

                string iconLink = _currencyService.GetCurrencyImageLink(normalizedCurrencyName);

                int stackSize;
                if (!int.TryParse(stackSizeValue.Value, out stackSize)) continue;

                var existingAnalysis = aiCurrencyAnalyses.FirstOrDefault(c => c.IconLink == iconLink);

                if (existingAnalysis != null)
                {
                    existingAnalysis.StackSize += stackSize;
                }
                else
                {
                    aiCurrencyAnalyses.Add(new AiCurrencyAnalysis
                    {
                        Text = string.IsNullOrEmpty(realCurrencyName) ? (string.IsNullOrEmpty(normalizedCurrencyName) ? currencyTypeValue.Value : normalizedCurrencyName) : realCurrencyName,
                        IconLink = iconLink,
                        StackSize = stackSize
                    });
                }

                chaosValue += normalizedCurrencyName == "chaos" ? stackSize : stackSize * _poeNinjaService.GetCurrencyChaosValue(CurrencyService.GetRealName(normalizedCurrencyName));
            }

            exaltedValue = chaosValue / _poeNinjaService.GetCurrencyChaosValue("Exalted Orb");

            OnTradeWindowScanned?.Invoke((float)chaosValue, (float)exaltedValue, aiCurrencyAnalyses, result);

            _runningAiAnalysis = false;
        }

        private void VerifyMapModifiers()
        {
            ClearSpecialKeys();
            FocusGame();
            SendCtrlC();

            var data = _clipboardService.GetClipboard(100);

            if (string.IsNullOrEmpty(data)) return;

            var mods = _itemService.FindMapModifiers(data);

            if (!mods.Any()) return;

            OnMapModifiersVerified(mods);
        }

        public void CloseAiServer()
        {
            _appAiService.ClosePythonServer();
        }

        public TradeWindowItem ParseTradeWindowItem(string data)
        {
            return _itemService.ParseTradeWindowItem(data);
        }

        public void MoveMouse(int x, int y)
        {
            _keyboardService.MoveMouse((uint)x, (uint)y);
        }

        public void MouseMoved()
        {
            OnMouseMoved?.Invoke();
        }

        public Point GetMousePosition()
        {
            return _keyboardService.GetMousePosition();
        }

        public void OnTradeAccepted()
        {
            NewChatEvent(ChatEventEnum.TradeAccepted);
        }

        public void OnTradeCancelled()
        {
            NewChatEvent(ChatEventEnum.TradeCancelled);
        }

        private void SearchItemInStash(bool useTypeInstead = false)
        {
            ClearSpecialKeys();
            FocusGame();
            SendCtrlC();

            var data = _clipboardService.GetClipboard(100);

            if (string.IsNullOrEmpty(data)) return;

            var (itemName, itemType) = _itemService.ParseItemNameAndType(data);

            var value = useTypeInstead && !string.IsNullOrEmpty(itemType) ? itemType : itemName;

            if (string.IsNullOrEmpty(value)) return;

            if (!_clipboardService.SetClipboard(value)) return;

            FocusGame();
            SendCtrlA();
            SendCtrlV();
        }

        private void DoShowTranslateInputControl()
        {
            OnShowTranslateInputControl();
        }

        public void ShareAiAnalysis(PredictionResponse response)
        {
            _cloudDataService.InsertAiAnalyzes(response);
        }

        public void TranslateMessage(string text, string targetLanguage = "", string sourceLanguage = "",
            bool notWhisper = false)
        {
            var messageMatch = Regex.Match(text, "\\$%@#");
            var isLocalMessage = !messageMatch.Success || (messageMatch.Success && messageMatch.Index != 0);
            var msgTag = isLocalMessage ? "" : text[..1];
            var playerIndex = text.IndexOf(" ", 1, StringComparison.Ordinal);
            var playerName = !notWhisper ? text.Substring(1, playerIndex - 1) : "";
            var msg = !notWhisper ? text[(playerIndex + 1)..] : isLocalMessage ? text : text[1..];

            var toLang = _translateService.LanguageToCode(targetLanguage);
            var fromLang = _translateService.LanguageToCode(sourceLanguage);

            if (_translationTable.ContainsKey(playerName))
            {
                toLang = _translateService.LanguageToCode(_translationTable[playerName]);
            }

            if (string.IsNullOrEmpty(toLang))
            {
                toLang = "en";
            }

            var translation = new ChatMessageTranslation()
            {
                MessageTag = msgTag,
                OriginalMessage = msg,
                PlayerName = playerName,
                UserInitiated = true,
                TranslationLang = toLang,
                Time = DateTime.Now
            };

            if (!string.IsNullOrEmpty(fromLang))
            {
                translation.OriginalLang = fromLang;
            }

            _ = _translateService.Translate(translation, new TranslateOptions() { To = toLang, From = fromLang });
        }

        private static void Shortcut_GoToHideout()
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

        private Area GetCurrentArea()
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

        private void ShowChaosRecipeOverlay()
        {
            OnToggleChaosRecipeOverlayVisibility?.Invoke(true);
        }

        private void HideChaosRecipeOverlay()
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

        private void NewTradeChatLine(TradeChatLine line)
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

        public void ChatScan(ChatMessageLogEntry entry)
        {
            if (!entry.Types.Contains(LogEntryType.Trade)) return;

            Task.Run(() =>
            {
                var chatScanWords = GetConfig().ChatScanWords;

                foreach (var unused in chatScanWords.Where(word =>
                    entry.Message.Contains(word) || entry.Player.Contains(word)))
                {
                    var line = HighlightScannedChatMessage(entry, chatScanWords);
                    NewTradeChatLine(line);
                }
            });
        }

        private static TradeChatLine HighlightScannedChatMessage(ChatMessageLogEntry entry, IEnumerable<string> words)
        {
            var tradeChatLine = new TradeChatLine(entry);
            var tradeWords = new List<TradeChatWords>();

            var message = entry.Message;

            foreach (var word in words)
            {
                int index;

                while ((index = message.IndexOf(word, StringComparison.Ordinal)) != -1)
                {
                    if (index == -1) continue;
                    var endIndex = index + word.Length;

                    if (endIndex > message.Length) continue;
                    tradeWords.Add(new TradeChatWords()
                    {
                        Highlighted = true,
                        Words = message.Substring(index, endIndex - index),
                        Index = index
                    });

                    message = $"{message[..index]}~{message[endIndex..]}";
                }
            }

            var currentIndex = 0;
            while (currentIndex < message.Length)
            {
                var nextIndex = message.IndexOf('~', currentIndex);

                if (nextIndex == -1)
                {
                    tradeWords.Add(new TradeChatWords()
                    {
                        Words = message[currentIndex..],
                        Index = currentIndex
                    });
                    break;
                }

                tradeWords.Add(new TradeChatWords()
                {
                    Words = message.Substring(currentIndex, nextIndex - currentIndex),
                    Index = currentIndex
                });

                currentIndex = nextIndex + 1;
            }

            tradeChatLine.Words = tradeWords.OrderBy(w => w.Index).ToList();

            return tradeChatLine;
        }

        private string GetClientFilePath()
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

        public TradeRequest CreateTradeRequest(Offer offer)
        {
            return PoeApiService.CreateTradeRequest(offer);
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
            return await PriceCheckingService.PriceCheck(offer, nbResults);
        }

        public double GetChaosValueOfCurrency(string currency)
        {
            return _poeNinjaService.GetCurrencyChaosValue(currency);
        }

        public double GetChaosValueOfRealNameCurrency(string currency)
        {
            return CurrencyService.GetChaosValue(currency);
        }

        public string GetCurrencyRealName(string currency)
        {
            return CurrencyService.GetRealName(currency);
        }

        public void SavePoeNinjaCaches(PoeNinjaCaches caches)
        {
            _appDataService.DeleteAllDocument(AppDataService.COLLECTION_POE_NINJA_CACHES);
            _appDataService.InsertDocument(AppDataService.COLLECTION_POE_NINJA_CACHES, caches);
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
                            OnOfferScam?.Invoke(priceCheck, offer);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                });
            }

            OnNewOffer?.Invoke(offer);
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

            _appDataService.UpdateDocument(AppDataService.COLLECTION_CONFIG, config);

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
            _appDataService.InsertDocument(AppDataService.COLLECTION_TRADES, offer);
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

        public void SendCtrlF()
        {
            ModifiedKeyStroke(Key.Control, Key.F);
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

        private void SendEscape()
        {
            _keyboardService.SendEscape();
        }

        public bool SetClipboard(string text)
        {
            return _clipboardService.SetClipboard(text);
        }

        public string GetClipboardValue(int delay = 0)
        {
            return _clipboardService.GetClipboard(delay);
        }

        public void ResetClipboardValue()
        {
            SetClipboard("");
        }

        public string ReplaceVars(string msg, Offer offer)
        {
            var currentArea = GetCurrentArea();

            msg = msg.Replace("{item}", offer.ItemName)
                .Replace("{price}", $"{offer.Price} {offer.Currency}")
                .Replace("{league}", offer.League)
                .Replace("{player}", offer.PlayerName);


            return currentArea != null
                ? msg.Replace("{location}", $"{currentArea.Name} ({currentArea.Type})")
                : msg.Replace("{location}", "Unknown location");
        }

        public IEnumerable<string> GetAvailableTranslationLanguages()
        {
            return TranslateService.GetLanguages();
        }

        public static void SendTradeChatCommand(string player)
        {
            ChatService.SendTradeCommand(player);
        }

        public static void SendHideoutChatCommand(string player)
        {
            ChatService.SendHideoutCommand(player);
        }

        private static void SendHideoutChatCommand()
        {
            ChatService.SendHideoutCommand();
        }

        public static void SendChatMessage(string msg, int delay = 0)
        {
            ChatService.SendChatMessage(msg, delay);
        }

        public static void SendKickChatCommand(string player)
        {
            ChatService.SendKickCommand(player);
        }

        public static void SendInviteChatCommand(string player)
        {
            ChatService.SendInviteCommand(player);
        }

        public static void HighlightStash(string text)
        {
            GameService.HighlightStash(text);
        }

        private void OnOnTextTranslated(ChatMessageTranslation translation)
        {
            OnTextTranslated?.Invoke(translation);
        }

        public void TextTranslated(ChatMessageTranslation translation)
        {
            switch (translation.UserInitiated)
            {
                case false when translation.OriginalLang == "English":
                    return;
                case false when !_translationTable.ContainsKey(translation.PlayerName):
                    _translationTable.Add(translation.PlayerName, translation.OriginalLang);
                    break;
                case false:
                    {
                        if (!string.Equals(_translationTable[translation.PlayerName], translation.OriginalLang,
                            StringComparison.Ordinal))
                        {
                            _translationTable[translation.PlayerName] = translation.OriginalLang;
                        }

                        break;
                    }
            }

            if (translation.UserInitiated)
            {
                SendChatMessage($"{translation.MessageTag}{translation.PlayerName} {translation.TranslatedMessage}");
            }
            else
            {
                OnOnTextTranslated(translation);
            }
        }

        private void OnShowTranslateInputControl()
        {
            ShowTranslateInputControl?.Invoke();
        }

        private void OnMapModifiersVerified(List<MapModifier> modifiers)
        {
            MapModifiersVerified?.Invoke(modifiers);
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
            _translateService.Start();
            _itemService.Start();
            _appAiService.Start();
            _screenCaptureService.Start();
            _cloudDataService.Start();
        }
    }
}