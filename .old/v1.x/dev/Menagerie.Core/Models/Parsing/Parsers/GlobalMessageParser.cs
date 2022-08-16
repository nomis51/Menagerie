using System.Collections.Generic;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public class GlobalMessageParser : ChatMessageParser
    {
        public GlobalMessageParser() : base(LogEntryType.Global, new Regex("#.+: .+", RegexOptions.IgnoreCase), new List<Regex>())
        {
        }
    }
}