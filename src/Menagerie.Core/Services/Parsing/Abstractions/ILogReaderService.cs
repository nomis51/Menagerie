using System.Collections.Generic;

namespace Menagerie.Core.Services.Parsing.Abstractions
{
    public interface ILogReaderService : IService
    {
        delegate void NewLogEntryEvent(string line);
        event NewLogEntryEvent NewLogEntry;
        
        
        string ReadLastLine();
        string ReadLine(int lineNo);
        IEnumerable<string> ReadLines(int[] linesNo);
    }
}