namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeResponse
{
    public string Id { get; set; }
    public string? Complexity { get; set; }
    public Dictionary<string, BulkTradeResult> Result { get; set; }
    public int Total { get; set; }
}