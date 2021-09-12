namespace Menagerie.Core.Models.Parsing.Entries
{
    public class AreaChangeLogEntry : LogEntry
    {
        public string Area { get; set; }

        public AreaChangeLogEntry(LogEntry entry) : base(entry.Raw, entry.Time, entry.Types, entry.Tag)
        {
        }
    }
}