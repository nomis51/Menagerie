using Menagerie.Core.Services.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Trading;

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

    private IClientFileService _clientFileService;
    private IClipboardService _clipboardService;
    private IGameProcessService _gameProcessService;
    private IGameWindowService _gameWindowService;
    private ITextParserService _textParserService;
    private IWindowHookService _windowHookService;

    #endregion

    #region Public methods

    public void ShowOverlay()
    {
        // DataEvents.OverlayVisibilityChangeEventInvoke(true);
    }

    public void HideOverlay()
    {
        // DataEvents.OverlayVisibilityChangeEventInvoke(false);
    }
    
    public void ClientFileFound(string filePath)
    {
        _clientFileService.SetClientFilePath(filePath);
    }
    
    public void IoHookProcess(int processId)
    {
        _windowHookService.IoHook(processId);
    }
    
    public void GameProcessFound(int processId)
    {
        _gameWindowService.SetProcessId(processId);
    }
    
    public void TradeAccepted()
    {
        // DataEvents.TradeAcceptedEventInvoke();
    }

    public void TradeCancelled()
    {
        // DataEvents.TradeCancelledEventInvoke();
    }

    public void PlayerJoined(string player)
    {
        // DataEvents.PlayerJoinedEventInvoke(player);
    }
    
    public void NewIncomingOffer(IncomingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);

        // DataEvents.NewIncomingOfferEventInvoke(offer);
    }

    public void NewOutgoingOffer(OutgoingOffer offer)
    {
        offer.Time = DateTime.Now;
        offer.CurrencyImageUri = new Uri(Path.GetFullPath(CurrencyHelper.GetCurrencyImageLink(offer.Currency)),
            UriKind.Absolute);

        // DataEvents.NewOutgoingOfferEventInvoke(offer);
    }
    
    public void OnLocationUpdated(string location)
    {
        // DataEvents.LocationUpdatedEventInvoke(location);
    }
    
     public void NewClipboardLine(string line)
    {
        if (_textParserService.CanParseKoreanOutgoingOffer(line))
        {
            _textParserService.ParseKoreanOutgoingOffer(line);
            // DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseRussianOutgoingOffer(line))
        {
            _textParserService.ParseRussianOutgoingOffer(line);
            // DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseFrenchOutgoingOffer(line))
        {
            _textParserService.ParseFrenchOutgoingOffer(line);
            // DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (_textParserService.CanParseGermanOutgoingOffer(line))
        {
            _textParserService.ParseGermanOutgoingOffer(line);
            // DataEvents.NewWhisperToSendEventInvoke(line);
            return;
        }

        if (!_textParserService.CanParseOutgoingOffer(line)) return;
        // DataEvents.NewWhisperToSendEventInvoke(line);
    }

    public void NewClientFileLine(string line)
    {
        _textParserService.ParseClientTxtLine(line);
    }

    public string GetLogsLocation()
    {
        return string.Empty;
    }

    public void Initialize()
    {
        _clientFileService = new ClientFileService();
        _clipboardService = new ClipboardService();
        _gameProcessService = new GameProcessService();
        _gameWindowService = new GameWindowService();
        _textParserService = new TextParserService();
        _windowHookService = new WindowHookService();
    }

    #endregion
}