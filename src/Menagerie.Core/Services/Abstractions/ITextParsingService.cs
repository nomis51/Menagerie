namespace Menagerie.Core.Services.Abstractions;

public interface ITextParsingService
{
    Task ParseIncomingTradeAsync(string line);
    Task ParseOutgoingTradeAsync(string line);
}