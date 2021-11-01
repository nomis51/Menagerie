using System.Collections.Generic;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public class TradeChatMessageParser : ChatMessageParser
    {
        public TradeChatMessageParser() : base(LogEntryType.Trade, new Regex("\\$.+: .+", RegexOptions.IgnoreCase), new List<Regex>())
        {
        }
    }
}