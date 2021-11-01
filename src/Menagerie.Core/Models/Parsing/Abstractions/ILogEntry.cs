using System;
using System.Collections.Generic;
using Menagerie.Core.Models.Parsing.Enums;

namespace Menagerie.Core.Models.Parsing.Abstractions
{
    public interface ILogEntry
    {
        string Raw { get; set; }
        DateTime Time { get; set; }
        IEnumerable<LogEntryType> Types { get; set; }
        LogEntryTag Tag { get; set; }
    }
}