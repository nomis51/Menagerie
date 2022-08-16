namespace Menagerie.Shared.Models.Poe.Trade;

public class Query
{
    public Filters Filters { get; set; }
    public List<Stat> Stats { get; set; } = new() { new Stat() };
    public Status Status { get; set; } = new();
    public string Type { get; set; }

    public Query(string type, string accountName = "")
    {
        Type = type;
        Filters = new Filters
        {
            TradeFilters = new TradeFilters
            {
                Filters = new TradeFiltersFilters
                {
                    Account = new Account
                    {
                        Input = accountName
                    }
                }
            }
        };
    }
}