using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Parsing;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Core.Parsers;

public class PlayerJoinedParser : Parser<PlayerJoinedChatEvent>
{
    public PlayerJoinedParser() : base(
        new Regex(" has joined the area", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new Token("] : ", false),
            new(" has joined the area.", true, "Player", typeof(string))
        }
    )
    {
    }
}