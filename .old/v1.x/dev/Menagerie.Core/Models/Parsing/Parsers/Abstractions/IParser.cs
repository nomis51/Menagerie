using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;

namespace Menagerie.Core.Models.Parsing.Parsers.Abstractions
{
    public interface IParser
    {
        bool CanParse(string line);
        ILogEntry Parse(string line, LogEntry entry);
        string Clean(string line);
        ILogEntry Execute(string line, LogEntry entry);
    }
}