namespace Menagerie.Shared.Models.Poe.Stash;

public class StashTabResponse
{
    public List<Item> Items { get;  } = new();
    public int NumTabs { get; set; }
    public List<StashTabResponseTab> Tabs { get; set; }
}