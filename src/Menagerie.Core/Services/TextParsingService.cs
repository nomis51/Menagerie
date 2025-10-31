using Menagerie.Core.Helpers.Parsing;
using Menagerie.Core.Models.Trading;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class TextParsingService : ITextParsingService
{
    #region Events

    public EventHandler<Trade>? NewIncomingTrade { get; set; }

    #endregion

    #region Members

    private readonly EnglishIncomingTradeWhisperParser _englishIncomingTradeWhisperParser = new();

    #endregion

    #region Public methods

    public void ParseIncomingTrade(string line)
    {
        var trade = _englishIncomingTradeWhisperParser.Parse(line);
        if (trade is null) return;

        NewIncomingTrade?.Invoke(this, trade);
    }

    public void ParseOutgoingTrade(string line)
    {
        throw new NotImplementedException();
    }

    #endregion
}