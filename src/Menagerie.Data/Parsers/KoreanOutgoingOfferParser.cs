using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Parsers;

public class KoreanOutgoingOfferParser : Parser<OutgoingOffer>
{
    #region Constructors

    // Need to take korean whisper right from the clipboard instead of client.txt
    public KoreanOutgoingOfferParser() : base(
        // item = fevered mind cobalt jewel 5 chaos
        // @PLAYER hello,    LEAGUE league excited  mind cobalt jewel_     PRICE_      buy     I would like to
        // @dokpla 안녕하세요, 파수꾼  리그의  흥분한    마음 코발트색 주얼(을)를 5 chaos(으)로 구매하고 싶습니다
        new Regex("@.+ 안녕하세요, .+ 리그의 .+\\(을\\)를 [0-9\\.]+ .+\\(으\\)로 구매하고 싶습니다",
            RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new ("@", false),
            new(" 안녕하세요, ", true, "Player", typeof(string)),
            new(" 리그의 ", true, "League", typeof(string)),
            new("(을)를 ", true, "ItemName", typeof(string)),
            new("(으)로 구매하고 싶습니다", true, "PriceStr", typeof(string))
        }
    )
    {
    }

    #endregion
}