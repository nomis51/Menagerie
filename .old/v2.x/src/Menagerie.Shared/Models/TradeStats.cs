namespace Menagerie.Shared.Models;

public class TradeStats
{
    public int NbTradesToday { get; set; }
    public double ChaosValueToday { get; set; }
    public double ExaltedValueToday { get; set; }
    public Dictionary<string, int> DateToNbTrades { get;  } = new();
    public Dictionary<string, double> DateToChaosValue { get;  } = new();
    public Dictionary<string, double> CurrencyTypeToAmount { get;  } = new();
    public Dictionary<string, int> ItemTypeToAmount { get;  } = new();
}