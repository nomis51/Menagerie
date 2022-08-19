using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using Menagerie.Data.Events;
using Menagerie.Data.Services;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Poe.Stash;
using Menagerie.Shared.Models.Setting;
using Menagerie.Shared.Models.Trading;
using Menagerie.Shared.Models.Translation;

namespace Menagerie.Application.Services;

public class AppService : IService
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

    #region Services

    private readonly GameChatService _gameChatService = new();
    private readonly AudioService _audioService = new();

    #endregion

    #region Constructors

    private AppService()
    {
        DataEvents.OnNewIncomingOffer += OnNewIncomingOffer;
        DataEvents.OnNewOutgoingOffer += OnNewOutgoingOffer;
        DataEvents.OnTradeAccepted += OnTradeAccepted;
        DataEvents.OnTradeCancelled += OnTradeCancelled;
        DataEvents.OnPlayerJoined += OnPlayerJoined;
        DataEvents.OnNewChaosRecipe += OnNewChaosRecipe;
        DataEvents.OnOverlayVisibilityChange += OnOverlayVisibilityChange;
        DataEvents.OnChatMessageFound += OnChatMessageFound;
        DataEvents.OnNewWhisperToSend += OnNewWhisperToSend;
        DataEvents.OnLocationUpdated += OnLocationUpdated;
        DataEvents.OnScamDetected += OnScamDetected;
        DataEvents.OnSearchOutgoingOffer += OnSearchOutgoingOffer;
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
        AppDataService.Instance.Initialize();
        _gameChatService.Initialize();
        _audioService.Initialize();
    }

    public async Task Start()
    {
        await AppDataService.Instance.Start();
        await _gameChatService.Start();
        await _audioService.Start();

        _ = UpdateHelper.UpdateApp();
    }

    public List<string> GetCurrencies()
    {
        return AppDataService.Instance.GetCurrencies();
    }

    public async Task<IEnumerable<BulkTradeItemDto>> SearchBulkTrade(string have, string want, int minWant = 1)
    {
        var response = await AppDataService.Instance.SearchBulkTrade(have, want, minWant);
        return response is null ? Enumerable.Empty<BulkTradeItemDto>() : AppMapper.Instance.MapBulkTradeResponse(response, minWant);
    }

    public void ToggleItemHighlight(bool isVisible)
    {
        AppEvents.HighlightItemEventInvoke(isVisible, 0, 0, 0, 0, string.Empty);
    }

    public void SaveLastClip()
    {
        AppDataService.Instance.SaveLastClip();
    }

    public void PlayNewOfferSoundEffect()
    {
        _audioService.PlayNewOfferSoundEffect();
    }

    public void PlayClickSoundEffect()
    {
        _audioService.PlayClickSoundEffect();
    }

    public void PlayPlayerJoinSoundEffect()
    {
        _audioService.PlayPlayerJoinSoundEffect();
    }

    public SettingsDto GetSettings()
    {
        return AppMapper.Instance.Map<SettingsDto>(AppDataService.Instance.GetSettings());
    }

    public void SetSettings(SettingsDto settings)
    {
        AppDataService.Instance.SetSettings(AppMapper.Instance.Map<Settings>(settings));
    }

    public bool EnsureGameFocused()
    {
        return AppDataService.Instance.EnsureGameFocused();
    }

    public bool EnsureOverlayFocused()
    {
        return AppDataService.Instance.EnsureOverlayFocused();
    }

    public void SetOverlayHandle(IntPtr handle)
    {
        AppDataService.Instance.SetOverlayHandle(handle);
    }

    public void OnAppExit()
    {
        AppDataService.Instance.OnAppExit();
    }

    public void SendBusyWhisper(string player, string itemName)
    {
        _gameChatService.SendBusyWhisper(player, itemName);
    }

    public void SendSoldWhisper(string player, string itemName)
    {
        _gameChatService.SendSoldWhisper(player, itemName);
    }

    public void SendStillInterestedWhisper(string player, string itemName, string price)
    {
        _gameChatService.SendStillInterestedWhisper(player, itemName, price);
    }

    public void SendInviteCommand(string player, string itemName, string price)
    {
        _gameChatService.SendInvite(player);
        _gameChatService.SendInviteWhisper(player, itemName, price);
    }

    public void SendKickCommand(string player)
    {
        _gameChatService.SendKick(player);
    }

    public void SendFindItemInStash(IncomingOfferDto offer)
    {
        var hasPosition = offer.Left != 0 && offer.Top != 0;

        if (hasPosition)
        {
            var settings = GetSettings();
            if (!settings.IncomingTrades.HighlightWithGrid)
            {
                _gameChatService.SendFindItemInStash($"pos:{offer.Left},{offer.Top}");
            }
            else
            {
                SaveStashTabGridSettings(offer.StashTab, 12, 12, false, true);
                AppEvents.HighlightItemEventInvoke(true, offer.Left, offer.Top, offer.Width, offer.Height, offer.StashTab);
            }
        }
        else
        {
            _gameChatService.SendFindItemInStash(offer.ItemNameNormalized);
        }
    }

    public void SaveStashTabGridSettings(string stashTab, int width, int height, bool hasFolderOffset, bool ensureCreated = false)
    {
        AppDataService.Instance.SaveStashTabGridSettings(stashTab, width, height, hasFolderOffset, ensureCreated);
    }

    public void SendReInvitecommand(string player)
    {
        _gameChatService.SendKick(player);
        _gameChatService.SendInvite(player);
    }

    public void SendTradeRequestCommand(string player)
    {
        _gameChatService.SendTradeRequest(player);
    }

    public void SendThanksWhisper(string player)
    {
        _gameChatService.SendThanksWhisper(player);
    }

    public void SendChatMessage(string message)
    {
        _gameChatService.Send(message);
    }

    public void InjectToClipboard(string text)
    {
        ClipboardHelper.SetClipboard(text);
    }

    public void SendHideoutCommand(string player = "")
    {
        _gameChatService.SendHideoutCommand(player);
    }

    public void SendLeavePartyCommand()
    {
        //TODO: find current character name to be able to kick it
        SendKickCommand("");
    }

    public void PrepareToSendWhisper(string player)
    {
        _gameChatService.PrepareToSendWhisper(player);
    }

    public Task<List<string>> GetLeagues()
    {
        return AppDataService.Instance.GetLeagues();
    }

    public void SaveTradeStatistic(IncomingOfferDto offer)
    {
        AppDataService.Instance.SaveTradeStatistic(AppMapper.Instance.Map<IncomingOffer>(offer));
    }

    public Task<string?> Translate(string text, string sourceLanguage, string targetLanguage)
    {
        var sourceLanguageCode = AppDataService.Instance.GetLanguageCode(sourceLanguage);
        var targetLanguageCode = AppDataService.Instance.GetLanguageCode(targetLanguage);
        return AppDataService.Instance.Translate(text, new TranslationOptions
        {
            FromLanguage = sourceLanguageCode,
            ToLanguage = targetLanguageCode
        });
    }

    public TradeStatsDto GetTradesStatistics()
    {
        return AppMapper.Instance.Map<TradeStatsDto>(AppDataService.Instance.GetTradesStatistics());
    }

    public IEnumerable<string> GetLanguages()
    {
        return AppDataService.Instance.GetLanguages();
    }

    public string GetCurrentLocation()
    {
        return AppDataService.Instance.CurrentLocation;
    }

    #endregion

    #region Private methods

    private void OnSearchOutgoingOffer()
    {
        AppEvents.SearchOutgoingOfferEventInvoke();
    }

    private void OnScamDetected(string id, string price)
    {
        AppEvents.ScamDetectedEventInvoke(id, price);
    }

    private void OnLocationUpdated(string location)
    {
        if (string.IsNullOrEmpty(location)) return;
        AppDataService.Instance.CurrentLocation = location;
    }

    private void OnNewWhisperToSend(string whisper)
    {
        _gameChatService.Send(whisper);
    }

    private void OnChatMessageFound(ChatMessage chatMessage)
    {
        AppEvents.ChatMessageFoundEventInvoke(AppMapper.Instance.Map<ChatMessageDto>(chatMessage));
    }

    private void OnOverlayVisibilityChange(bool isVisible)
    {
        AppEvents.OverlayVisibilityChangeEventInvoke(isVisible);
    }

    private void OnNewChaosRecipe(ChaosRecipe chaosRecipe)
    {
        AppEvents.NewChaosRecipeEventInvoke(AppMapper.Instance.Map<List<ChaosRecipeItemDto>>(chaosRecipe));
    }

    private void OnPlayerJoined(string player)
    {
        AppEvents.PlayerJoinedEventInvoke(player);
    }

    private void OnNewOutgoingOffer(OutgoingOffer offer)
    {
        AppEvents.NewOutgoingOfferEventInvoke(AppMapper.Instance.Map<OutgoingOfferDto>(offer));
    }

    private void OnTradeCancelled()
    {
        AppEvents.TradeCancelledEventInvoke();
    }

    private void OnTradeAccepted()
    {
        AppEvents.TradeAcceptedEventInvoke();
    }

    private void OnNewIncomingOffer(IncomingOffer offer)
    {
        AppEvents.NewIncomingOfferEventInvoke(AppMapper.Instance.Map<IncomingOfferDto>(offer));
    }

    #endregion
}