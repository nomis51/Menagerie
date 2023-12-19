namespace Menagerie.Shared.Models.Poe.Trade;

public class Stat
{
    public List<string> Filters { get; set; } = new();
    public string Type { get; set; } = "and";
}