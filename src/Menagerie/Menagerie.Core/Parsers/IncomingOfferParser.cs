using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Core.Parsers;

public class IncomingOfferParser : Parser<IncomingOffer>
{
    #region Constructors

    public IncomingOfferParser() : base(
        new Regex("@From .+: Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9\\.]+ .+ in .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            // (stash tab "~price 1 chaos"; position: left 8, top 7)
            new("@From ", false),
            new("<", false, breakOnFail: false),
            new("> ", false, breakOnFail: false),
            new(": Hi, ", true, "Player", typeof(string)),
            new("I would", false, "", null, "I'd"),
            new(" like to buy your ", false),
            new(" listed for ", true, "ItemName", typeof(string), " for my "),
            new(" in ", true, "PriceStr", typeof(double)),
            new(" (stash tab \"", true, "League", typeof(string)),
            new("\"; position: left ", true, "StashTab", typeof(string)),
            new(", top ", true, "LeftStr", typeof(string)),
            new(")", true, "TopStr", typeof(string))
        }
    )
    {
    }

    #endregion
}