using Menagerie.Data.Events;
using Menagerie.Data.Providers;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Poe.BulkTrade;
using Menagerie.Shared.Models.Poe.Stash;
using Menagerie.Shared.Models.Setting;
using Menagerie.Shared.Models.Trading;
using Menagerie.Shared.Models.Translation;
using Serilog;

namespace Menagerie.Data.Services;

public class AppDataService : IService
{
    #region Singleton

    private static readonly object LockInstance = new object();
    private static AppDataService _instance;

    public static AppDataService Instance
    {
        get
        {
            lock (LockInstance)
            {
                _instance ??= new AppDataService();
            }

            return _instance;
        }
    }

    #endregion

    #region Props

    public string CurrentLocation { get; set; } = string.Empty;

    #endregion

    #region Services

    private readonly GameProcessService _gameProcessService;
    private readonly GameWindowService _gameWindowService;
    private readonly ClientFileService _clientFileService;
    private readonly TextParserService _textParserService;
    private readonly SettingsService _settingsService;
    private readonly PoeNinjaService _poeNinjaService;
    private readonly ClipboardService _clipboardService;
    private readonly PoeApiService _poeApiService;
    private readonly WindowHookService _windowHookService;
    private readonly TranslationService _translationService;

    #endregion

    #region Constructors

    private AppDataService()
    {
        _gameProcessService = new GameProcessService();
        _gameWindowService = new GameWindowService();
        _clientFileService = new ClientFileService();
        _textParserService = new TextParserService();
        _settingsService = new SettingsService();
        _poeNinjaService = new PoeNinjaService();
        _clipboardService = new ClipboardService();
        _poeApiService = new PoeApiService();
        _windowHookService = new WindowHookService();
        _translationService = new TranslationService();
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
        InitializeLogs();
        InitializeDatabaseProvider();
        EnsureCleanStart();

        Log.Information("Initializing data services...");

        _settingsService.Initialize();
        _gameProcessService.Initialize();
        _gameWindowService.Initialize();
        _clientFileService.Initialize();
        _textParserService.Initialize();
        _poeNinjaService.Initialize();
        _clipboardService.Initialize();
        _poeApiService.Initialize();
        _windowHookService.Initialize();
        _translationService.Initialize();
    }

    public Task Start()
    {
        Log.Information("Starting data services...");

        _ = _settingsService.Start();
        _ = _gameProcessService.Start();
        _ = _gameWindowService.Start();
        _ = _clientFileService.Start();
        _ = _textParserService.Start();
        _ = _poeNinjaService.Start();
        _ = _clipboardService.Start();
        _ = _poeApiService.Start();
        _ = _windowHookService.Start();
        _ = _translationService.Start();

        return Task.CompletedTask;
    }

    public List<string> GetCurrencies()
    {
        return CurrencyHelper.GetRealCurrencyNames();
    }

    public async Task<BulkTradeResponse?> SearchBulkTrade(string have, string want, int minimum = 1)
    {
        return await _poeApiService.FetchBulkTrade(new BulkTradeRequest
        {
            Query = new BulkTradeQuery
            {
                Have = new[] { CurrencyHelper.NormalizeCurrency(have) },
                Want = new[] { CurrencyHelper.NormalizeCurrency(want) },
                Minimum = minimum
            }
        });
    }

    public Settings GetSettings()
    {
        return _settingsService.GetSettings();
    }

    public void SetSettings(Settings settings)
    {
        _settingsService.SetSettings(settings);
    }

    public void SetOverlayHandle(IntPtr handle)
    {
        _gameWindowService.SetOverlayHandle(handle);
    }

    public void OnAppExit()
    {
        DataEvents.ApplicationExitEventInvoke();
        ProcessHelper.OnExit();
        DatabaseProvider.OnExit();
    }

    public void SaveStashTabGridSettings(string stashTab, int width, int height, bool hasFolderOffset, bool ensureCreated = false)
    {
        var settings = GetSettings();
        var gridSettingsIndex = settings.StashTabGrid.TabsGridSettings.FindIndex(g => g.StashTab == stashTab);

        if (gridSettingsIndex != -1 && ensureCreated) return;

        if (gridSettingsIndex == -1)
        {
            settings.StashTabGrid.TabsGridSettings.Add(new GridSettings
            {
                StashTab = stashTab,
                Width = width,
                Height = height,
                HasFolderOffset = hasFolderOffset
            });
        }
        else
        {
            settings.StashTabGrid.TabsGridSettings[gridSettingsIndex].Width = width;
            settings.StashTabGrid.TabsGridSettings[gridSettingsIndex].Height = height;
            settings.StashTabGrid.TabsGridSettings[gridSettingsIndex].HasFolderOffset = hasFolderOffset;
        }

        SetSettings(settings);
    }

    public void GameProcessFound(int processId)
    {
        _gameWindowService.SetProcessId(processId);
    }

    public void ClientFileFound(string filePath)
    {
        _clientFileService.SetClientFilePath(filePath);
    }

    public void IoHookProcess(int processId)
    {
        _windowHookService.IoHook(processId);
    }

    public void OnSearchOutgoingOffer()
    {
        DataEvents.SearchOutgoingOfferEventInvoke();
    }

    public void OnSearchItemInStash()
    {
        KeyboardHelper.SendControlC();
        var text = ClipboardHelper.GetClipboardValue(50);
        var itemName = ItemHelper.ExtractItemName(text);
        if (string.IsNullOrEmpty(itemName)) return;

        ClipboardHelper.SetClipboard(itemName, 100);

        KeyboardHelper.ClearModifiers();
        KeyboardHelper.SendControlV();
        KeyboardHelper.SendEnter();
    }

    public void OnLocationUpdated(string location)
    {
        DataEvents.LocationUpdatedEventInvoke(location);
    }

    public void NewClientFileLine(string line)
    {
        _textParserService.ParseClientTxtLine(line);
    }

    public Task<string?> Translate(string text, TranslationOptions options)
    {
        return _translationService.Translate(text, options);
    }

    public string GetLanguageCode(string language)
    {
        return _translationService.LanguageToCode(language);
    }

    public IEnumerable<string> GetLanguages()
    {
        return _translationService.GetLanguages();
    }

    public void NewClipboardLine(string line)
    {
        if (_textParserService.CanParseKoreanOutgoingOffer(line))
        {
            _ = _textParserService.ParseKoreanOutgoingOffer(line);
            DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseRussianOutgoingOffer(line))
        {
            _ = _textParserService.ParseRussianOutgoingOffer(line);
            DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseFrenchOutgoingOffer(line))
        {
            _ = _textParserService.ParseFrenchOutgoingOffer(line);
            DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseGermanOutgoingOffer(line))
        {
            _ = _textParserService.ParseGermanOutgoingOffer(line);
            DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (!_textParserService.CanParseOutgoingOffer(line)) return;
        DataEvents.NewWhisperToSendEventInvoke(line);
    }

    public void NewIncomingOffer(IncomingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);
        offer.PriceConversions =
            new PriceConversions(_poeNinjaService.CalculatePriceConversions(offer.Price, offer.Currency));
        var (width, height) = _poeApiService.GetItemSize(offer.ItemName);
        offer.Width = width;
        offer.Height = height;

        var settings = GetSettings();

        if (settings.IncomingTrades.VerifyPrice)
        {
            Task.Run(async () =>
            {
                var apiPrice = await _poeApiService.VerifyPrice(offer.ItemName, $"{offer.Price} {CurrencyHelper.NormalizeCurrency(offer.Currency)}");
                if (string.IsNullOrEmpty(apiPrice)) return;
                if (offer.PriceStr == apiPrice) return;

                DataEvents.ScamDetectedEventInvoke(offer.Id, apiPrice);
            });
        }

        DataEvents.NewIncomingOfferEventInvoke(offer);
    }

    public void NewOutgoingOffer(OutgoingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);
        offer.PriceConversions =
            new PriceConversions(_poeNinjaService.CalculatePriceConversions(offer.Price, offer.Currency));
        var itemImage = _poeApiService.GetItemImageLink(offer.ItemName);
        offer.ImageUri = string.IsNullOrEmpty(itemImage) ? null : new Uri(itemImage);

        DataEvents.NewOutgoingOfferEventInvoke(offer);
    }

    public void TradeAccepted()
    {
        DataEvents.TradeAcceptedEventInvoke();
    }

    public void TradeCancelled()
    {
        DataEvents.TradeCancelledEventInvoke();
    }

    public void PlayerJoined(string player)
    {
        DataEvents.PlayerJoinedEventInvoke(player);
    }

    public bool EnsureGameFocused()
    {
        return _gameWindowService.FocusGameWindow();
    }

    public bool EnsureOverlayFocused()
    {
        return _gameWindowService.FocusOverlay();
    }

    public void NewChaosRecipe(ChaosRecipe chaosRecipe)
    {
        DataEvents.NewChaosRecipeEventInvoke(chaosRecipe);
    }

    public void ShowOverlay()
    {
        DataEvents.OverlayVisibilityChangeEventInvoke(true);
    }

    public void HideOverlay()
    {
        DataEvents.OverlayVisibilityChangeEventInvoke(false);
    }

    public void ToggleOverlay()
    {
        _gameWindowService.ToggleOverlay();
    }

    public void ChatMessageFound(ChatMessage chatMessage)
    {
        DataEvents.ChatMessageFoundEventInvoke(chatMessage);
    }

    public Task<List<string>> GetLeagues()
    {
        return _poeApiService.FetchLeagues();
    }

    public double GetChaosValueOf(double value, string currency)
    {
        return _poeNinjaService.CalculateChaosValue(value, currency);
    }

    public double GetExaltedValue(double chaosValue)
    {
        return _poeNinjaService.CalculateExaltedValue(chaosValue);
    }

    #endregion

    #region Private methods

    private void EnsureCleanStart()
    {
        ProcessHelper.CleanUnexpectedProcesses();
    }

    private void InitializeLogs()
    {
        LogHelper.Initialize();
    }

    private void InitializeDatabaseProvider()
    {
        DatabaseProvider.Initialize();
    }

    #endregion
}