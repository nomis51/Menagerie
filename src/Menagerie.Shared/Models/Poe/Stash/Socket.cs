using Newtonsoft.Json;

namespace Menagerie.Shared.Models.Poe.Stash;

public class Socket
{
    [JsonProperty("sColour")]
    public string Colour { get; set; }
}