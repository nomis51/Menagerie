using System.Text.RegularExpressions;
using Menagerie.Core.Parsers;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class TextParserService : ITextParserService
{
    #region Members

    private readonly Regex RegOutgoingWhisper = new(
        "@.+ Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly Dictionary<string, string> _leagueTranslationToRealLeagueName = new()
    {
        { "sentinel", "Sentinel" },
        { "warden", "Sentinel" },
    };

    private readonly IncomingOfferParser _incomingOfferParser;
    private readonly OutgoingOfferParser _outgoingOfferParser;
    private readonly TradeAcceptedParser _tradeAcceptedParser;
    private readonly TradeCancelledParser _tradeCancelledParser;
    private readonly PlayerJoinedParser _playerJoinedParser;
    private readonly RussianOutgoingOfferParser _russianOutgoingOfferParser;
    private readonly KoreanOutgoingOfferParser _koreanOutgoingOfferParser;
    private readonly FrenchOutgoingOfferParser _frenchOutgoingOfferParser;
    private readonly GermanOutgoingOfferParser _germanOutgoingOfferParser;
    private readonly LocationParser _locationParser;

    #endregion

    #region Constructors

    public TextParserService()
    {
        _incomingOfferParser = new IncomingOfferParser();
        _outgoingOfferParser = new OutgoingOfferParser();
        _tradeAcceptedParser = new TradeAcceptedParser();
        _tradeCancelledParser = new TradeCancelledParser();
        _playerJoinedParser = new PlayerJoinedParser();
        _russianOutgoingOfferParser = new RussianOutgoingOfferParser();
        _koreanOutgoingOfferParser = new KoreanOutgoingOfferParser();
        _frenchOutgoingOfferParser = new FrenchOutgoingOfferParser();
        _germanOutgoingOfferParser = new GermanOutgoingOfferParser();
        _locationParser = new LocationParser();
    }

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public string GetRealLeagueName(string translatedLeagueName)
    {
        var lowerTranslatedLeagueName = translatedLeagueName.ToLower().Trim();
        return _leagueTranslationToRealLeagueName.GetValueOrDefault(lowerTranslatedLeagueName, translatedLeagueName);
    }

    public void ParseClientTxtLine(string text)
    {
        if (_locationParser.CanParse(text))
        {
            var location = _locationParser.Parse(text);
            if (location is null) return;

            AppService.Instance.OnLocationUpdated(location.Location);

            return;
        }

        if (_incomingOfferParser.CanParse(text))
        {
            var offer = _incomingOfferParser.Parse(text);
            if (offer is null) return;

            offer.Whisper = text;
            AppService.Instance.NewIncomingOffer(offer);
            return;
        }

        if (_outgoingOfferParser.CanParse(text))
        {
            var offer = _outgoingOfferParser.Parse(text);
            if (offer is null) return;

            offer.Whisper = text;
            AppService.Instance.NewOutgoingOffer(offer);
            return;
        }

        if (_tradeAcceptedParser.CanParse(text))
        {
            _ = _tradeAcceptedParser.Parse(text);
            AppService.Instance.TradeAccepted();
            return;
        }

        if (_tradeCancelledParser.CanParse(text))
        {
            _ = _tradeCancelledParser.Parse(text);
            AppService.Instance.TradeCancelled();
        }

        if (_playerJoinedParser.CanParse(text))
        {
            var playerJoinedEvent = _playerJoinedParser.Parse(text);
            if (playerJoinedEvent is null) return;
            AppService.Instance.PlayerJoined(playerJoinedEvent.Player);
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

    public void ParseGermanOutgoingOffer(string text)
    {
        var offer = _germanOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        offer.Whisper = text;
        AppService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseFrenchOutgoingOffer(string text)
    {
        return _frenchOutgoingOfferParser.CanParse(text);
    }

    public void ParseFrenchOutgoingOffer(string text)
    {
        var offer = _frenchOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        offer.Whisper = text;
        AppService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseRussianOutgoingOffer(string text)
    {
        return _russianOutgoingOfferParser.CanParse(text);
    }

    public void ParseRussianOutgoingOffer(string text)
    {
        var offer = _russianOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        offer.Whisper = text;
        AppService.Instance.NewOutgoingOffer(offer);
    }

    public bool CanParseKoreanOutgoingOffer(string text)
    {
        return _koreanOutgoingOfferParser.CanParse(text);
    }

    public void ParseKoreanOutgoingOffer(string text)
    {
        var offer = _koreanOutgoingOfferParser.Parse(text);
        if (offer is null) return;

        offer.Whisper = text;
        AppService.Instance.NewOutgoingOffer(offer);
    }

    #endregion
}