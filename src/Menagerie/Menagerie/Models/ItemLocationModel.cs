namespace Menagerie.Models;

public class ItemLocationModel
{
    public string StashTab { get; set; }
    public int Left { get; set; }
    public int Top { get; set; }

    public ItemLocationModel(string stashTab, int left, int top)
    {
        StashTab = stashTab;
        Left = left;
        Top = top;
    }
}