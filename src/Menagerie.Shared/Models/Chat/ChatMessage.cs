namespace Menagerie.Shared.Models.Chat;

public class ChatMessage : Entity
{
    #region Constants

    public const string GlobalMessageType = "Global";
    public const string TradeMessageType = "Trade";
    private const string UnknownMessageType = "Unknown";

    #endregion

    public string Player { get; set; }
    public string Type { get; set; } = UnknownMessageType;
    public string Message { get; set; }
    public DateTime Time { get; set; }
}