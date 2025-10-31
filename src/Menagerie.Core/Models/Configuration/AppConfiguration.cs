using System.Text.Json.Serialization;

namespace Menagerie.Core.Models.Configuration;

public class AppConfiguration
{
    [JsonPropertyName("game")]
    public GameAppConfiguration Game { get; init; } = new();

    [JsonPropertyName("clipboard")]
    public ClipboardAppConfiguration Clipboard { get; init; } = new();

    [JsonPropertyName("clientLog")]
    public ClientLogAppConfiguration ClientLog { get; init; } = new();
}