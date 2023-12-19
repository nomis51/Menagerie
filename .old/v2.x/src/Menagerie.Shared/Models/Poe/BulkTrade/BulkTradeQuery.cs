namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeQuery
{
    public BulkTradeStatus Status { get; set; } = new();
    public IEnumerable<string> Want { get; set; }
    public IEnumerable<string> Have { get; set; }
    public int Minimum { get; set; }
}