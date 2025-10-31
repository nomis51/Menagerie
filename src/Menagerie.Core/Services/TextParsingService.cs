using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class TextParsingService : ITextParsingService
{
    #region Public methods

    public Task ParseIncomingTradeAsync(string line)
    {
        throw new NotImplementedException();
    }

    public Task ParseOutgoingTradeAsync(string line)
    {
        throw new NotImplementedException();
    }

    #endregion
}