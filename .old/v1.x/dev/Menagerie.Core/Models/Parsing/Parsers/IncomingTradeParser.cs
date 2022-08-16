using System.Collections.Generic;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public class IncomingTradeParser : TradeParser
    {
        public IncomingTradeParser() : base(LogEntryType.Incoming, new Regex(
            "@From .+: Hi, (I would|I'd) like to buy your .+ (listed for|for my) [0-9]+ .+ in .+",
            RegexOptions.IgnoreCase), new List<Regex>
        {
            new Regex("@From ", RegexOptions.IgnoreCase),
            new Regex(": Hi, (I would|I'd) like to buy your ", RegexOptions.IgnoreCase),
            new Regex(" (listed for|for my) ", RegexOptions.IgnoreCase),
            new Regex(" in ", RegexOptions.IgnoreCase),
            new Regex(" \\(stash tab \"", RegexOptions.IgnoreCase),
            new Regex("\"; position: left ", RegexOptions.IgnoreCase),
            new Regex(", top ", RegexOptions.IgnoreCase),
        })
        {
        }
    }
}