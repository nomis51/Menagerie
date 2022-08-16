namespace Menagerie.Core.Models.Parsing.Entries
{
    public class ChatMessageLogEntry : LogEntry
    {
        public string Player { get; set; }
        public string Message { get; set; }
        
        public ChatMessageLogEntry(LogEntry entry) : base(entry.Raw, entry.Time, entry.Types, entry.Tag)
        {
        }
    }
}