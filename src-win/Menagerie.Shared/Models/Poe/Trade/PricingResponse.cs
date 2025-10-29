namespace Menagerie.Shared.Models.Poe.Trade;

public class PricingResponse
{
    public int Complexity { get; set; }
    public string Id { get; set; }
    public int Total { get; set; }
    public IEnumerable<string> Result { get; set; } = new List<string>();   
}