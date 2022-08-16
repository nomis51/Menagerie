namespace Menagerie.Shared.Models.Poe.Trade;

public class PricingRequest
{
    public Query Query { get; set; }
    public Sort Sort { get; set; } = new();

    public PricingRequest(string type, string accountName)
    {
        Query = new Query(type, accountName);
    }
}