using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Core.Parsers;

public class FrenchOutgoingOfferParser : Parser<OutgoingOffer>
{
    #region Constructors

    public FrenchOutgoingOfferParser() : base(
        new Regex("@.+ Salut, je voudrais t'acheter .+ contre [0-9\\.]+ .+ \\(ligue .+\\)\\.",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("@", false),
            new(" Salut, je voudrais t'acheter ", true, "Player", typeof(string)),
            new(" contre ", true, "ItemName", typeof(string)),
            new(" (ligue ", true, "PriceStr", typeof(string)),
            new(").", true, "League", typeof(string)),
        }
    )
    {
    }

    #endregion
}