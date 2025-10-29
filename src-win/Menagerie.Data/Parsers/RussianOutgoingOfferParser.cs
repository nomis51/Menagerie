using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Parsers;

public class RussianOutgoingOfferParser : Parser<OutgoingOffer>
{
    #region Constructors

    public RussianOutgoingOfferParser() : base(
        // @To .+: hello, want buy and you .+ behind .+ in league .+
        new Regex("@.+ Здравствуйте, хочу купить у вас .+ за [0-9\\.]+ .+ в лиге .+",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("@", false),
            new(" Здравствуйте, ", true, "Player", typeof(string)),
            new("хочу купить у вас ", false, ""),
            new(" за ", true, "ItemName", typeof(string)),
            new(" в лиге ", true, "PriceStr", typeof(double)),
            new(".", true, "League", typeof(string), endOfString: true)
        }
    )
    {
    }

    #endregion
}