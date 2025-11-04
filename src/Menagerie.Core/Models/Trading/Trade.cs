using System.Text;
using Menagerie.Core.Enums.Trading;
using Menagerie.Core.Models.Database.Abstractions;

namespace Menagerie.Core.Models.Trading;

public class Trade : DbEntity
{
    #region Props

    public required DateTime Time { get; init; }
    public required string ItemName { get; set; }
    public required string PlayerName { get; init; }
    public required string League { get; init; }
    public required float Price { get; set; }
    public required string Currency { get; init; }
    public required StashTabLocation StashTab { get; init; }
    public TradeState State { get; set; } = TradeState.Initial;
    public required TradeType Type { get; init; }
    public required string Whisper { get; init; }

    #endregion

    #region Public methods

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"Id: {Id}");
        sb.AppendLine($"Time: {Time:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"ItemName: {ItemName}");
        sb.AppendLine($"PlayerName: {PlayerName}");
        sb.AppendLine($"League: {League}");
        sb.AppendLine($"Price: {Price}");
        sb.AppendLine($"Currency: {Currency}");
        sb.AppendLine($"StashTab: {StashTab}");
        sb.AppendLine($"State: {State}");
        sb.AppendLine($"Type: {Type}");
        sb.AppendLine($"Whisper: {Whisper}");

        return sb.ToString();
    }

    #endregion
}