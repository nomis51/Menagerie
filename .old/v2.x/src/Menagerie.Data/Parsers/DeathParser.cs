using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Parsing;

namespace Menagerie.Data.Parsers;

public class DeathParser : Parser<DeathEvent>
{
    public DeathParser() : base(
        new Regex(".+ has been slain\\.", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("] : ", false),
            new (" has been slain.", true, "CharacterName", typeof(string))
        }
    )
    {
    }
}