namespace Menagerie.Shared.Models.Trading;

public class PriceConversions
{
    public string Text { get; set; }
    public int ChaosValue { get; set; }
    public double ExaltedValue { get; set; }

    public PriceConversions(Tuple<string, int, double> values)
    {
        Text = values.Item1;
        ChaosValue = values.Item2;
        ExaltedValue = values.Item3;
    }
    
    public PriceConversions(){}
}