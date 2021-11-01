using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public class PlayerJoinedAreaParser : Parser
    {
        private readonly List<Regex> _regsClean = new List<Regex>()
        {
            new Regex(": ", RegexOptions.IgnoreCase),
            new Regex(" has joined the area.", RegexOptions.IgnoreCase)
        };

        private readonly Regex _regMatch = new Regex(".+ has joined the area.", RegexOptions.IgnoreCase);


        public override bool CanParse(string line)
        {
            return _regMatch.IsMatch(line);
        }

        public override ILogEntry Parse(string line, LogEntry entry)
        {
            return new PlayerJoinedAreaLogEntry(entry)
            {
                Types = new[] {LogEntryType.System, LogEntryType.Party, LogEntryType.JoinArea},
                Player = line
            };
        }

        public override string Clean(string line)
        {
            return _regsClean.Aggregate(line, (current, reg) => reg.Replace(current, ""));
        }
    }
}