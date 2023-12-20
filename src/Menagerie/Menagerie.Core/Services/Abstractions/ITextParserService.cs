namespace Menagerie.Core.Services.Abstractions;

public interface ITextParserService : IService
{
    void ParseClientTxtLine(string line);
    string GetRealLeagueName(string translatedLeagueName);
    bool CanParseOutgoingOffer(string text);
    bool CanParseGermanOutgoingOffer(string text);
    void ParseGermanOutgoingOffer(string text);
    bool CanParseFrenchOutgoingOffer(string text);
    void ParseFrenchOutgoingOffer(string text);
    bool CanParseRussianOutgoingOffer(string text);
    void ParseRussianOutgoingOffer(string text);
    bool CanParseKoreanOutgoingOffer(string text);
    void ParseKoreanOutgoingOffer(string text);
}