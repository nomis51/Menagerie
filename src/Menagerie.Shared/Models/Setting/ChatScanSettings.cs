namespace Menagerie.Shared.Models.Setting;

public class ChatScanSettings
{
    public bool Enabled { get; set; }
    public List<string> Words { get; set; }
    public bool AutoRemoveMessage { get; set; }
    public int AutoRemoveMessageDelay { get; set; }
}