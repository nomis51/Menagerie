using System.Text.Json.Serialization;

namespace Menagerie.Core.Models.Configuration;

public class AppConfiguration
{
    [JsonPropertyName("game")]
    public GameAppConfiguration Game { get; set; } = new();
}