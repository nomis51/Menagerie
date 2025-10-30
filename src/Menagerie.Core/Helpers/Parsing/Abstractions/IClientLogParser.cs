namespace Menagerie.Core.Helpers.Parsing.Abstractions;

public interface IClientLogParser<out T>
    where T : class
{
    T? Parse(string line);
}