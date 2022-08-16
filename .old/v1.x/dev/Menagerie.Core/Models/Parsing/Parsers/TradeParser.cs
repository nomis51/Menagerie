using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;
using Menagerie.Core.Services.Parsing;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public abstract class TradeParser : Parser
    {
        private readonly Regex _regNumber = new Regex("[0-9]+", RegexOptions.IgnoreCase);
        private readonly Regex _regMatch;
        private readonly List<Regex> _regsClean;
        private readonly LogEntryType _type;
        private readonly CurrencyService _currencyService;

        protected TradeParser(LogEntryType type, Regex regMatch, List<Regex> regsClean)
        {
            _type = type;
            _regMatch = regMatch;
            _regsClean = regsClean;
            _currencyService = new CurrencyService();
        }

        public override bool CanParse(string line)
        {
            return _regMatch.IsMatch(line);
        }

        public override ILogEntry Parse(string line, LogEntry entry)
        {
            var tradeLogEntry = new TradeLogEntry(entry)
            {
                Types = new List<LogEntryType>
                {
                    _type,
                    LogEntryType.Trade,
                    LogEntryType.Whisper
                }
            };

            var parts = line.Split("#", StringSplitOptions.RemoveEmptyEntries);

            tradeLogEntry.Player = Regex.IsMatch(parts[0], "<.+> .+") ? parts[0][(parts[0].IndexOf("> ", StringComparison.Ordinal) + 2)..] : parts[0];

            var itemMatch = _regNumber.Match(parts[1]);

            if (itemMatch.Success && itemMatch.Index == 0 && itemMatch.Length == parts[1].Length)
            {
                var itemParts = parts[1].Split(" ");

                tradeLogEntry.Item = new Item()
                {
                    Quantity = Convert.ToInt32(itemParts[0]),
                    Name = itemParts[1..].Aggregate((total, value) => $"{total} {value}")
                };
            }
            else
            {
                tradeLogEntry.Item = new Item()
                {
                    Name = parts[1]
                };
            }

            var priceParts = parts[2].Split(" ");
            var realCurrencyName = _currencyService.GetRealName(priceParts[1]);
            var loweredCurrencyName = priceParts[1].ToLower();

            tradeLogEntry.Price = new Price()
            {
                Value = Convert.ToDouble(priceParts[0]),
                Currency = realCurrencyName,
                NormalizedCurrency = loweredCurrencyName,
                ImageLink = _currencyService.GetCurrencyImageLink(loweredCurrencyName)
            };

            tradeLogEntry.League = parts[3];

            if (parts.Length > 4)
            {
                tradeLogEntry.Location = new StashLocation()
                {
                    StashTab = parts[4],
                    Left = Convert.ToInt32(parts[5]),
                    Top = Convert.ToInt32(parts[6].EndsWith(")") ? parts[6][..^1] : parts[6])
                };
            }

            return tradeLogEntry;
        }

        public override string Clean(string line)
        {
            return _regsClean.Aggregate(line.EndsWith(".") ? line[..^1] : line,
                (current, reg) => reg.Replace(current, "#"));
        }
    }
}