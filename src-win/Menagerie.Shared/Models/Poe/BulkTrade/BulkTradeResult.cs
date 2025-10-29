namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeResult
{
    public string Id { get; set; }
    public string? Item { get; set; }
    public BulkTradeListing Listing { get; set; }
}