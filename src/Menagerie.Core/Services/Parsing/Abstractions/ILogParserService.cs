using Menagerie.Core.Models.Parsing.Abstractions;
using Menagerie.Core.Models.Parsing.Parsers.Abstractions;

namespace Menagerie.Core.Services.Parsing.Abstractions
{
    public interface ILogParserService : IService
    {
        ILogEntry Parse(string line);
        void AddParser(IParser parser);
        void RemoveAllParsers();
    }
}