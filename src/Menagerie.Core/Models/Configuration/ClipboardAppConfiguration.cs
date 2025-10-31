using System.Text.Json.Serialization;

namespace Menagerie.Core.Models.Configuration;

public class ClipboardAppConfiguration
{
    [JsonPropertyName("pollingRate")]
    public TimeSpan PollingRate { get; init; } = TimeSpan.FromMilliseconds(500);

    [JsonPropertyName("exclusionQueueLength")]
    public int ExclusionQueueLength { get; init; } = 10;

    [JsonPropertyName("nbRetriesOnFail")]
    public int NbRetriesOnFail { get; init; } = 3;

    [JsonPropertyName("internalDelay")]
    public TimeSpan InternalDelay { get; init; } = TimeSpan.FromMilliseconds(50);
}