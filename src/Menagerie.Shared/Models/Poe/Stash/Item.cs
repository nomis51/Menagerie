using Newtonsoft.Json;

namespace Menagerie.Shared.Models.Poe.Stash;

public class Item
{
    [JsonProperty("w")]
    public int Width { get; set; }

    [JsonProperty("h")]
    public int Height { get; set; }

    [JsonProperty("icon")]
    public string IconUrl { get; set; }

    [JsonProperty("typeLine")]
    public string Type { get; set; }

    public string Name { get; set; }

    [JsonProperty("ilvl")]
    public int ItemLevel { get; set; }

    public int FrameType { get; set; }

    [JsonProperty("artFilename")]
    public string ArtFileName { get; set; }

    public List<Property> Properties { get;  } = new();
    public List<Requirement> Requirements { get;  } = new();
    public List<Socket> Sockets { get;  } = new();
    public List<string> EnchantMods { get;  }= new();
    public List<string> ImplicitMods { get;  }= new();
    public List<string> ExplicitMods { get;  }= new();
    public List<string> CraftedMods { get;  }= new();
    public List<LogbookModifier> LogbookMods { get;  } = new();
    
    public bool Searing { get; set; }
    public bool Tangled { get; set; }
}