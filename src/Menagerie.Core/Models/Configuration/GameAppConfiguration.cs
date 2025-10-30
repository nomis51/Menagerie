using System.Text.Json.Serialization;

namespace Menagerie.Core.Models.Configuration;

public class GameAppConfiguration
{
    [JsonPropertyName("processNames")]
    public List<string> ProcessNames { get; set; } =
    [
        "PathOfExile",
        "PathOfExile_Steam",
        "PathOfExile_x64",
        "PathOfExile_x64Steam",
        "PathOfExileSteam",
        "PathOfExileStea", // NOT a typo
    ];

    [JsonPropertyName("waitTimeIfNotFound")]
    public TimeSpan WaitTimeIfNotFound { get; set; } = TimeSpan.FromSeconds(10);
}