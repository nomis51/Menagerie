using Menagerie.Core.Models.Trading;

namespace Menagerie.Core.Services.Abstractions;

public interface ITextParsingService
{
    EventHandler<Trade>? NewIncomingTrade { get; set; }
    void ParseIncomingTrade(string line);
    void ParseOutgoingTrade(string line);
}