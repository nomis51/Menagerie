namespace Menagerie.Application.DTOs;

public class ChaosRecipeItemDto
{
    public string Label { get; set; }
    public int Count { get; set; }
    public string IconLink { get; set; }
    
    public ChaosRecipeItemDto(string label, int count, string iconLink = "")
    {
        Label = label;
        Count = count;
        IconLink = iconLink;
    }
}