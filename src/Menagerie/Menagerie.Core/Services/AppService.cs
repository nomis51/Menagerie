using Menagerie.Core.Services.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Setting;
using Menagerie.Shared.Models.Trading;
using Serilog;

namespace Menagerie.Core.Services;

public class AppService : IAppService
{
    #region Singleton

    private static readonly object LockInstance = new();
#pragma warning disable CS8618
    private static AppService _instance;
#pragma warning restore CS8618

    public static AppService Instance
    {
        get
        {
            lock (LockInstance)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                _instance ??= new AppService();
            }

            return _instance;
        }
    }

    #endregion

    #region Services

    private IClientFileService _clientFileService = null!;
    private IClipboardService _clipboardService = null!;
    private IGameProcessService _gameProcessService = null!;
    private IGameWindowService _gameWindowService = null!;
    private ITextParserService _textParserService = null!;
    private ISettingsService _settingsService = null!;
    private IGameChatService _gameChatService = null!;

    #endregion

    #region Props

    public string CurrentLocation { get; private set; } = string.Empty;

    #endregion

    #region Public methods

    public Settings GetSettings()
    {
        return _settingsService.GetSettings();
    }

    public void SetSettings(Settings settings)
    {
        _settingsService.SetSettings(settings);
    }


    public bool EnsureGameFocused()
    {
        return _gameWindowService.FocusGameWindow();
    }

    public void ShowOverlay()
    {
        Events.ShowOverlayEventInvoke();
    }

    public void HideOverlay()
    {
        Events.HideOverlayEventInvoke();
    }

    public void ClientFileFound(string filePath)
    {
        _clientFileService.SetClientFilePath(filePath);
    }

    public void GameProcessFound(int processId)
    {
        _gameWindowService.SetProcessId(processId);
    }

    public void TradeAccepted()
    {
        Events.TradeAcceptedEventInvoke();
    }

    public void TradeCancelled()
    {
        Events.TradeCancelledEventInvoke();
    }

    public void PlayerJoined(string player)
    {
        Events.PlayerJoinedEventInvoke(player);
    }

    public void NewIncomingOffer(IncomingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);

        Events.NewIncomingOfferEventInvoke(offer);
    }

    public void NewOutgoingOffer(OutgoingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);

        Events.NewOutgoingOfferEventInvoke(offer);
    }

    public void OnLocationUpdated(string location)
    {
        Events.LocationUpdatedEventInvoke(location);
    }

    public void NewClipboardLine(string line)
    {
        if (_textParserService.CanParseKoreanOutgoingOffer(line))
        {
            _textParserService.ParseKoreanOutgoingOffer(line);
            _gameChatService.Send(line);
            return;
        }

        if (_textParserService.CanParseRussianOutgoingOffer(line))
        {
            _textParserService.ParseRussianOutgoingOffer(line);
            _gameChatService.Send(line);
            return;
        }

        if (_textParserService.CanParseFrenchOutgoingOffer(line))
        {
            _textParserService.ParseFrenchOutgoingOffer(line);
            _gameChatService.Send(line);
            return;
        }

        if (_textParserService.CanParseGermanOutgoingOffer(line))
        {
            _textParserService.ParseGermanOutgoingOffer(line);
            _gameChatService.Send(line);
            return;
        }

        if (!_textParserService.CanParseOutgoingOffer(line)) return;
        _gameChatService.Send(line);
    }

    public void NewClientFileLine(string line)
    {
        _textParserService.ParseClientTxtLine(line);
    }

    public void Initialize()
    {
        LogsHelper.Initialize();
        ProcessHelper.CleanUnexpectedProcesses();

        _clientFileService = new ClientFileService();
        _clipboardService = new ClipboardService();
        _gameProcessService = new GameProcessService();
        _gameWindowService = new GameWindowService();
        _textParserService = new TextParserService();
        _settingsService = new SettingsService();
        _gameChatService = new GameChatService();

        _gameProcessService.FindProcess();
        _clipboardService.Listen();
    }

    #endregion
}