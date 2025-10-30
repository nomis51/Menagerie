using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Models.Database.Abstractions;

namespace Menagerie.Core.Models.Trading;

public class Trade : DbEntity
{
    public required DateTime Time { get; set; }
    public required string ItemName { get; set; }
    public required string PlayerName { get; set; }
    public required string League { get; set; }
    public required float Price { get; set; }
    public required string Currency { get; set; }
    public required StashTabLocation StashTab { get; set; }
    public TradeState State { get; set; } = TradeState.Initial;
    public required TradeType Type { get; set; }
    public required string Whisper { get; set; }
}