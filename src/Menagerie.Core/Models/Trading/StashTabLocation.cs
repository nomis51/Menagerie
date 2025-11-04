using System.Text;

namespace Menagerie.Core.Models.Trading;

public class StashTabLocation
{
    #region Props

    public required string? Name { get; init; }
    public required int Left { get; init; }
    public required int Top { get; init; }

    #endregion

    #region Public methods

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.AppendLine($"Name: {Name}");
        sb.AppendLine($"Left: {Left}");
        sb.AppendLine($"Top: {Top}");

        return sb.ToString();
    }

    #endregion
}