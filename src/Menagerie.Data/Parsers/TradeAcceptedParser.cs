using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Parsers;

public class TradeAcceptedParser : Parser<TradeAcceptedChatEvent>
{
    #region Constructors

    public TradeAcceptedParser() : base(
        new Regex("Trade accepted", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>()
    )
    {
    }

    #endregion
}