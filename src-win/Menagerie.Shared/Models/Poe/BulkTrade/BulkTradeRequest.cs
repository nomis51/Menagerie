namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeRequest
{
    public BulkTradeQuery Query { get; set; } = new();
 
    public BulkTradeSort Sort { get; set; } = new();
    public string Engine { get; set; } = "new";
}