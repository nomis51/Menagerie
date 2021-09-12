using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public class AreaChangeParser : Parser
    {
        private readonly Regex _regMatch = new Regex("You have entered .+\\.", RegexOptions.IgnoreCase);

        private readonly List<Regex> _regsClean = new List<Regex>()
        {
            new Regex(": ", RegexOptions.IgnoreCase),
            new Regex("You have entered ", RegexOptions.IgnoreCase),
            new Regex("\\.", RegexOptions.IgnoreCase)
        };

        public override bool CanParse(string line)
        {
            return _regMatch.IsMatch(line);
        }

        public override ILogEntry Parse(string line, LogEntry entry)
        {
            return new AreaChangeLogEntry(entry) {Types = new[] {LogEntryType.System, LogEntryType.ChangeArea}, Area = line};
        }

        public override string Clean(string line)
        {
            return _regsClean.Aggregate(line, (current, reg) => reg.Replace(current, ""));
        }
    }
}