using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Core.Parsers;

public class OutgoingOfferParser : Parser<OutgoingOffer>
{
    #region Constructors

    public OutgoingOfferParser() : base(
        new Regex("@To .+: Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("@To ", false),
            new("<", false, breakOnFail: false),
            new("> ", false, breakOnFail: false),
            new(": Hi, ", true, "Player", typeof(string)),
            new("I would", false, "", null, "I'd"),
            new(" like to buy your ", false),
            new(" listed for ", true, "ItemName", typeof(string), " for my "),
            new(" in ", true, "PriceStr", typeof(double)),
            new(".", true, "League", typeof(string), endOfString: true)
        }
    )
    {
    }

    #endregion
}