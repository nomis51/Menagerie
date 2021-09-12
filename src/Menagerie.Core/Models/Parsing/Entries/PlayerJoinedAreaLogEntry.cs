namespace Menagerie.Core.Models.Parsing.Entries
{
    public class PlayerJoinedAreaLogEntry : LogEntry
    {
        public string Player { get; set; }

        public PlayerJoinedAreaLogEntry(LogEntry entry) : base(entry.Raw, entry.Time, entry.Types, entry.Tag)
        {
        }
    }
}