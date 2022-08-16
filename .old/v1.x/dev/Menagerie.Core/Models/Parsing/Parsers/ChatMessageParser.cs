using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public abstract class ChatMessageParser : Parser
    {
        private readonly Regex _regMatch;

        private readonly List<Regex> _regsClean;
        private readonly LogEntryType _type;

        protected ChatMessageParser(LogEntryType type, Regex regMatch, List<Regex> regsClean)
        {
            _type = type;
            _regMatch = regMatch;
            _regsClean = regsClean;
        }

        public override bool CanParse(string line)
        {
            return _regMatch.IsMatch(line);
        }

        public override ILogEntry Parse(string line, LogEntry entry)
        {
            var chatMessageLogEntry = new ChatMessageLogEntry(entry)
            {
                Types = new[] {LogEntryType.ChatMessage, _type}
            };

            var parts = line.Split(": ", StringSplitOptions.RemoveEmptyEntries);
            chatMessageLogEntry.Player = Regex.Replace(parts[0], "[#@%$]", "");
            chatMessageLogEntry.Message = parts[1..].Aggregate((total, value) => total + value);

            return chatMessageLogEntry;
        }

        public override string Clean(string line)
        {
            return line;
        }
    }
}