namespace Menagerie.Shared.Models.Poe.Trade;

public class ItemData
{
    public string Name { get; set; }
    public string Namespace { get; set; }
    public string Icon { get; set; }
    public Craftable? Craftable { get; set; }
    public Unique? Unique { get; set; }
}

public class Craftable
{
    public string Category { get; set; }
}

public class Unique
{
    public string Base { get; set; }
}