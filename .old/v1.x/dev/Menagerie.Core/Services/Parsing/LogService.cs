using System.Collections.Generic;
using System.Linq;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Enums;
using Menagerie.Core.Models.Parsing.Parsers.Abstractions;
using Menagerie.Core.Services.Parsing.Abstractions;

namespace Menagerie.Core.Services.Parsing
{
    public class LogService : ILogService
    {
        #region Events

        public event ILogService.NewLogEntryEvent NewLogEntry;
        public event ILogService.NewTradeLogEntryEvent NewTradeLogEntry;
        public event ILogService.NewPlayerJoinedAreaLogEntryEvent NewPlayerJoinedAreaLogEntry;
        public event ILogService.NewAreaChangeLogEntryEvent NewAreaChangeLogEntry;
        public event ILogService.NewChatMessageLogEntryEvent NewChatMessageLogEntry;
        public event ILogService.NewTradeStateLogEntryEvent NewTradeStateLogEntry;

        #endregion

        #region Members

        private readonly ILogReaderService _logReaderService;
        private readonly ILogParserService _logParserService;

        #endregion

        public LogService(string logFilePath = "")
        {
            _logReaderService = string.IsNullOrEmpty(logFilePath)
                ? new LogReaderService()
                : new LogReaderService(logFilePath);
            _logParserService = new LogParserService();

            _logReaderService.NewLogEntry += LogReaderService_OnNewLogEntry;
        }

        private void LogReaderService_OnNewLogEntry(string line)
        {
            if (string.IsNullOrEmpty(line)) return;

            var entry = _logParserService.Parse(line);

            if (entry == null) return;

            if (entry.Types.Contains(LogEntryType.ChatMessage))
            {
                OnNewChatMessageLogEntry((ChatMessageLogEntry) entry);
            }
            else if (entry.Types.Contains(LogEntryType.TradeState))
            {
                OnNewTradeStateLogEntry((TradeStateLogEntry) entry);
            }
            else if (entry.Types.Contains(LogEntryType.Trade))
            {
                OnNewTradeLogEntry((TradeLogEntry) entry);
            }
            else if (entry.Types.Contains(LogEntryType.ChangeArea))
            {
                OnNewAreaChangeLogEntry((AreaChangeLogEntry) entry);
            }
            else if (entry.Types.Contains(LogEntryType.JoinArea))
            {
                OnNewPlayerJoinedAreaLogEntry((PlayerJoinedAreaLogEntry) entry);
            }
            else
            {
                OnNewLogEntry(entry);
            }
        }

        public void AddParser(IParser parser)
        {
            _logParserService.AddParser(parser);
        }

        public void AddParsers(IEnumerable<IParser> parsers)
        {
            var enumerable = parsers as IParser[] ?? parsers.ToArray();

            if (!enumerable.Any()) return;

            foreach (var parser in enumerable)
            {
                _logParserService.AddParser(parser);
            }
        }

        public void RemoveAllParsers()
        {
            _logParserService.RemoveAllParsers();
        }

        public ILogEntry ReadLastLine()
        {
            var line = _logReaderService.ReadLastLine();

            return string.IsNullOrEmpty(line) ? default(ILogEntry) : _logParserService.Parse(line);
        }

        public ILogEntry ReadLine(int lineNo)
        {
            if (lineNo < 0) return default(ILogEntry);

            var line = _logReaderService.ReadLine(lineNo);

            return string.IsNullOrEmpty(line) ? default(ILogEntry) : _logParserService.Parse(line);
        }

        public Dictionary<int, ILogEntry> ReadLines(int[] linesNo)
        {
            var entries = new Dictionary<int, ILogEntry>();
            var lines = _logReaderService.ReadLines(linesNo).ToList();

            for (var i = 0; i < linesNo.Length; ++i)
            {
                entries.Add(linesNo[i], _logParserService.Parse(lines[i]));
            }

            return entries;
        }

        public ILogEntry Parse(string line)
        {
            return _logParserService.Parse(line);
        }

        #region Event invokers

        protected virtual void OnNewLogEntry(ILogEntry logEntry)
        {
            NewLogEntry?.Invoke(logEntry);
        }

        protected virtual void OnNewTradeLogEntry(TradeLogEntry logEntry)
        {
            NewTradeLogEntry?.Invoke(logEntry);
        }

        protected virtual void OnNewPlayerJoinedAreaLogEntry(PlayerJoinedAreaLogEntry logEntry)
        {
            NewPlayerJoinedAreaLogEntry?.Invoke(logEntry);
        }

        protected virtual void OnNewAreaChangeLogEntry(AreaChangeLogEntry logEntry)
        {
            NewAreaChangeLogEntry?.Invoke(logEntry);
        }

        protected virtual void OnNewChatMessageLogEntry(ChatMessageLogEntry logEntry)
        {
            NewChatMessageLogEntry?.Invoke(logEntry);
        }

        #endregion

        protected virtual void OnNewTradeStateLogEntry(TradeStateLogEntry logEntry)
        {
            NewTradeStateLogEntry?.Invoke(logEntry);
        }
    }
}