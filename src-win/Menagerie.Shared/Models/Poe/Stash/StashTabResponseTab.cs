using Newtonsoft.Json;

namespace Menagerie.Shared.Models.Poe.Stash;

public class StashTabResponseTab
{
    [JsonProperty("n")]
    public string Name { get; set; }

    [JsonProperty("i")]
    public int Index { get; set; }

    public string Type { get; set; }
    public bool Hidden { get; set; }
    public bool Selected { get; set; }

    [JsonProperty("colour")]
    public TabColor Color { get; set; }

    public string League { get; set; }
}