namespace Menagerie.Shared.Models.Poe.Stash;

public class LogbookModifier
{
    public string Name { get; set; }
    public LogbookFaction Faction { get; set; }
    public List<string> Mods { get;  } = new();
}