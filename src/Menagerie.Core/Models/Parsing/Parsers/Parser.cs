using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Entries;
using Menagerie.Core.Models.Parsing.Parsers.Abstractions;

namespace Menagerie.Core.Models.Parsing.Parsers
{
    public abstract class Parser : IParser
    {
        public abstract bool CanParse(string line);

        public abstract ILogEntry Parse(string line, LogEntry entry);

        public abstract string Clean(string line);

        public ILogEntry Execute(string line, LogEntry entry)
        {
            line = Clean(line);
            return Parse(line, entry);
        }
    }
}