using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Parsers;

public class TradeCancelledParser : Parser<TradeCancelledChatEvent>
{
    #region Constructors

    public TradeCancelledParser() : base(
        new Regex("Trade cancelled", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>()
    )
    {
    }

    #endregion
}