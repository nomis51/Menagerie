namespace Menagerie.Shared.Models.Poe.Trade;

public class FetchItemsResponse
{
   public IEnumerable<FetchItem> Result { get;  } = new List<FetchItem>();
}