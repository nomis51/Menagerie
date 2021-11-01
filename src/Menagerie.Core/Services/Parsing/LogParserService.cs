using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;
using Menagerie.Core.Models.Parsing.Parsers;
using Menagerie.Core.Models.Parsing.Parsers.Abstractions;
using Menagerie.Core.Services.Parsing.Abstractions;

namespace Menagerie.Core.Services.Parsing
{
    public class LogParserService : ILogParserService
    {
        private readonly CurrencyService _currencyService;
        private List<IParser> _parsers;

        private readonly Regex _regLogEntryTag =
            new Regex("\\[(debug|warn|info) Client [0-9]+\\] ", RegexOptions.IgnoreCase);

        public LogParserService()
        {
            _currencyService = new CurrencyService();
            _parsers = new List<IParser>()
            {
                new IncomingTradeParser(),
                new OutgoingTradeParser(),
                new AreaChangeParser(),
                new PlayerJoinedAreaParser(),
                new TradeStateParser(),
                new TradeChatMessageParser(),
                new GlobalMessageParser(),
            };
        }

        public ILogEntry Parse(string line)
        {
            var entry = ParseLogEntry(line);

            if (entry == null)
            {
                return default(LogEntry);
            }

            foreach (var parser in _parsers.Where(parser => parser.CanParse(line)))
            {
                try
                {
                    return parser.Execute(entry.Item2, entry.Item1);
                }
                catch
                {
                    // ignored
                }
            }

            entry.Item1.Types = new[] { LogEntryType.System };

            return entry.Item1;
        }

        private Tuple<LogEntry, string> ParseLogEntry(string line)
        {
            var entry = new LogEntry(line, DateTime.Now, new List<LogEntryType>());

            string timeStr;

            if (line.StartsWith("@"))
            {
                timeStr = DateTime.Now.ToString();
            }
            else
            {
                var dateEndIndex = line.IndexOf(" ", StringComparison.Ordinal);

                if (dateEndIndex == -1)
                {
                    return default;
                }

                var timeEndIndex = line.IndexOf(" ", dateEndIndex, StringComparison.Ordinal);

                if (timeEndIndex == -1)
                {
                    return default;
                }

                var dateTimeEndIndex = (dateEndIndex + timeEndIndex) - 1;

                if (dateTimeEndIndex == -1) return default;

                timeStr = line[..dateTimeEndIndex];
                line = line[(dateTimeEndIndex + 1)..];
            }

            var logTagMatch = _regLogEntryTag.Match(line);

            if (logTagMatch.Success)
            {

                var tagStr = line.Substring(logTagMatch.Index + 1,
                    line.IndexOf(" ", logTagMatch.Index, StringComparison.Ordinal));
                tagStr = tagStr[..tagStr.IndexOf(" ", StringComparison.Ordinal)];
                line = line[(logTagMatch.Index + logTagMatch.Length)..];

                entry.Tag = LogEntryTagConveter.Convert(tagStr);
            }

            entry.Time = DateTime.Parse(timeStr);

            return new Tuple<LogEntry, string>(entry, line);
        }

        public void AddParser(IParser parser)
        {
            _parsers.Add(parser);
        }

        public void RemoveAllParsers()
        {
            _parsers.Clear();
        }
    }
}