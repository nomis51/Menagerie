using System.Diagnostics;
using System.Text.RegularExpressions;
using Menagerie.Data.Parsers;
using Menagerie.Data.Parsers.Abstractions;
using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Models;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Trading;
using Menagerie.Shared.Models.Translation;

namespace Menagerie.Data.Services;

public class TextParserService : IService
{
    #region Members

    private readonly Regex RegOutgoingWhisper = new(
        "@.+ Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Dictionary<string, string> _leagueTranslationToRealLeagueName = new()
    {
        { "sentinel", "Sentinel" },
        { "warden", "Sentinel" }
    };

    private readonly IncomingOfferParser _incomingOfferParser;
    private readonly OutgoingOfferParser _outgoingOfferParser;
    private readonly TradeAcceptedParser _tradeAcceptedParser;
    private readonly TradeCancelledParser _tradeCancelledParser;
    private readonly PlayerJoinedParser _playerJoinedParser;
    private readonly TradeChatParser _tradeChatParser;
    private readonly GlobalChatParser _globalChatParser;
    private readonly RussianOutgoingOfferParser _russianOutgoingOfferParser;
    private readonly KoreanOutgoingOfferParser _koreanOutgoingOfferParser;
    private readonly FrenchOutgoingOfferParser _frenchOutgoingOfferParser;
    private readonly GermanOutgoingOfferParser _germanOutgoingOfferParser;
    private readonly LocationParser _locationParser;
    private readonly DeathParser _deathParser;

    #endregion

    #region Constructors

    public TextParserService()
    {
        _incomingOfferParser = new IncomingOfferParser();
        _outgoingOfferParser = new OutgoingOfferParser();
        _tradeAcceptedParser = new TradeAcceptedParser();
        _tradeCancelledParser = new TradeCancelledParser();
        _playerJoinedParser = new PlayerJoinedParser();
        _tradeChatParser = new TradeChatParser();
        _globalChatParser = new GlobalChatParser();
        _russianOutgoingOfferParser = new RussianOutgoingOfferParser();
        _koreanOutgoingOfferParser = new KoreanOutgoingOfferParser();
        _frenchOutgoingOfferParser = new FrenchOutgoingOfferParser();
        _germanOutgoingOfferParser = new GermanOutgoingOfferParser();
        _locationParser = new LocationParser();
        _deathParser = new DeathParser();
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        return Task.CompletedTask;
    }

    public string GetRealLeagueName(string translatedLeagueName)
    {
        var lowerTranslatedLeagueName = translatedLeagueName.ToLower().Trim();
        if (_leagueTranslationToRealLeagueName.ContainsKey(lowerTranslatedLeagueName))
            return _leagueTranslationToRealLeagueName[lowerTranslatedLeagueName];
        return translatedLeagueName;
    }

    public void ParseClientTxtLine(string text)
    {
        if (_locationParser.CanParse(text))
        {
            var location = _locationParser.Parse(text);
            if (location is null) return;

            AppDataService.Instance.OnLocationUpdated(location.Location);

            return;
        }

        if (_incomingOfferParser.CanParse(text))
        {
            var offer = _incomingOfferParser.Parse(text);
            if (offer is null) return;

            offer.Whisper = text;
            AppDataService.Instance.NewIncomingOffer(offer);
            return;
        }

        if (_outgoingOfferParser.CanParse(text))
        {
            var offer = _outgoingOfferParser.Parse(text);
            if (offer is null) return;

            offer.Whisper = text;
            AppDataService.Instance.NewOutgoingOffer(offer);
            return;
        }

        if (_tradeAcceptedParser.CanParse(text))
        {
            _ = _tradeAcceptedParser.Parse(text);
            AppDataService.Instance.TradeAccepted();
            return;
        }

        if (_tradeCancelledParser.CanParse(text))
        {
            _ = _tradeCancelledParser.Parse(text);
            AppDataService.Instance.TradeCancelled();
        }

        if (_playerJoinedParser.CanParse(text))
        {
            var playerJoinedEvent = _playerJoinedParser.Parse(text);
            if (playerJoinedEvent is null) return;
            AppDataService.Instance.PlayerJoined(playerJoinedEvent.Player);
        }

        if (_deathParser.CanParse(text))
        {
            var deathEvent = _deathParser.Parse(text);
            if (deathEvent is null) return;
            AppDataService.Instance.PlayerDied(deathEvent.CharacterName);
        }

        if (_tradeChatParser.CanParse(text))
        {
            var chatMessage = _tradeChatParser.Parse(text);
            if (chatMessage is null) return;

            chatMessage.Type = ChatMessage.TradeMessageType;
            AppDataService.Instance.NewChatMessage(chatMessage);
        }

        if (_globalChatParser.CanParse(text))
        {
            var chatMessage = _globalChatParser.Parse(text);
            if (chatMessage is null) return;

            chatMessage.Type = ChatMessage.GlobalMessageType;
            AppDataService.Instance.NewChatMessage(chatMessage);
        }
    }

    public bool CanParseOutgoingOffer(string text)
    {
        var match = RegOutgoingWhisper.Match(text);
        return match is not null && match.Success && match.Length == text.Length;
    }
    
    public bool CanParseGermanOutgoingOffer(string text)
    {
        return _germanOutgoingOfferParser.CanParse(text);
    }

    public async Task ParseGermanOutgoingOffer(string text)
    {
        var offer = _germanOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        TranslationOptions translationOtions = new()
        {
            FromLanguage = "de",
            ToLanguage = "en"
        };

        offer.Whisper = text;
        offer.ItemName = (await AppDataService.Instance.Translate(offer.ItemName, translationOtions)) ?? string.Empty;
        offer.League = GetRealLeagueName((await AppDataService.Instance.Translate(offer.League, translationOtions)) ??
                                         string.Empty);
        AppDataService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseFrenchOutgoingOffer(string text)
    {
        return _frenchOutgoingOfferParser.CanParse(text);
    }

    public async Task ParseFrenchOutgoingOffer(string text)
    {
        var offer = _frenchOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        TranslationOptions translationOtions = new()
        {
            FromLanguage = "fr",
            ToLanguage = "en"
        };

        offer.Whisper = text;
        offer.ItemName = (await AppDataService.Instance.Translate(offer.ItemName, translationOtions)) ?? string.Empty;
        offer.League = GetRealLeagueName((await AppDataService.Instance.Translate(offer.League, translationOtions)) ??
                                         string.Empty);
        AppDataService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseRussianOutgoingOffer(string text)
    {
        return _russianOutgoingOfferParser.CanParse(text);
    }

    public async Task ParseRussianOutgoingOffer(string text)
    {
        var offer = _russianOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        TranslationOptions translationOtions = new()
        {
            FromLanguage = "ru",
            ToLanguage = "en"
        };

        offer.Whisper = text;
        offer.ItemName = (await AppDataService.Instance.Translate(offer.ItemName, translationOtions)) ?? string.Empty;
        offer.League = GetRealLeagueName((await AppDataService.Instance.Translate(offer.League, translationOtions)) ??
                                         string.Empty);
        AppDataService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseKoreanOutgoingOffer(string text)
    {
        return _koreanOutgoingOfferParser.CanParse(text);
    }

    public async Task ParseKoreanOutgoingOffer(string text)
    {
        var offer = _koreanOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        TranslationOptions translationOtions = new()
        {
            FromLanguage = "ko",
            ToLanguage = "en"
        };

        offer.Whisper = text;
        offer.ItemName = (await AppDataService.Instance.Translate(offer.ItemName, translationOtions)) ?? string.Empty;
        offer.League = GetRealLeagueName((await AppDataService.Instance.Translate(offer.League, translationOtions)) ??
                                         string.Empty);
        AppDataService.Instance.NewOutgoingOffer(offer);
    }

    #endregion
}