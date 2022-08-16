using System;
using System.Collections.Generic;
using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Entries
{
    public class LogEntry : ILogEntry
    {
        public string Raw { get; set; }
        public DateTime Time { get; set; }
        public IEnumerable<LogEntryType> Types { get; set; }
        public LogEntryTag Tag { get; set; }

        public LogEntry(string raw, DateTime time, IEnumerable<LogEntryType> types,
            LogEntryTag tag = LogEntryTag.Info)
        {
            Raw = raw;
            Time = time;
            Types = types;
            Tag = tag;
        }
    }
}