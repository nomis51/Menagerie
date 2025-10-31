using System.Text.Json.Serialization;

namespace Menagerie.Core.Models.Configuration;

public class ClientLogAppConfiguration
{
    [JsonPropertyName("pollingRate")]
    public TimeSpan PollingRate { get; init; } = TimeSpan.FromMilliseconds(500);
}