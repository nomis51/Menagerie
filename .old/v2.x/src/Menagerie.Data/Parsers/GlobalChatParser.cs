using System.Text.RegularExpressions;
using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Parsing;

namespace Menagerie.Data.Parsers;

public class GlobalChatParser : Parser<ChatMessage>
{
    public GlobalChatParser() : base(
        new Regex("#.+: ", RegexOptions.Compiled | RegexOptions.IgnoreCase),
        new List<Token>
        {
            new("#", false, "Type", typeof(string)),
            new(": ", true, "Player", typeof(string)),
            new("", true, "Message", typeof(string), endOfString:true)
        }
    )
    {
    }
}