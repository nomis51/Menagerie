using Menagerie.Core.Models.Parsing.Abstractions;

namespace Menagerie.Core.Models.Parsing.Entries
{
    public class TradeStateLogEntry : LogEntry
    {
        public bool IsAccepted { get; set; }
        public bool IsCancelled { get; set; }

        public TradeStateLogEntry(ILogEntry entry) : base(entry.Raw, entry.Time, entry.Types, entry.Tag)
        {
        }
    }
}