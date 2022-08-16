namespace Menagerie.Shared.Models.Setting;

public class StashTabGridSettings
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int IncrementValue { get; set; }
    public int HighlightBorderThickness { get; set; }
    public bool DropShadow { get; set; }
    public double DropShadowOpacity { get; set; }
    public int FolderOffset { get; set; }
    public List<GridSettings> TabsGridSettings { get; set; } = new();
}

public class GridSettings
{
    public string StashTab { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public bool HasFolderOffset { get; set; }
}