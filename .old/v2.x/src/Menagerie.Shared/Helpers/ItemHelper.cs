using Newtonsoft.Json;
using Serilog;

namespace Menagerie.Shared.Helpers;

public static class ItemHelper
{
    #region Constants

    private static readonly List<string> WeaponsNames = new() { "wand", "scepter", "dagger", "rune dagger", "claw" };
    private const string OneHandedName = "one-handed";
    private const string TwoHandedName = "one-handed";

    #endregion

    #region Members

    private static Dictionary<string, string> _itemCategories = new();
    private static readonly Dictionary<string, string> ItemCategoryImages = new();

    #endregion

    #region Public methods

    public static string GetItemCategory(string itemName)
    {
        if (!_itemCategories.Any())
        {
            ReadItemCategories();
        }

        var nonSuperiorName = itemName.Replace("Superior ", "");

        return _itemCategories.ContainsKey(nonSuperiorName) ? _itemCategories[nonSuperiorName] : string.Empty;
    }

    public static string GetItemCategoryImageLink(string category)
    {
        if (!ItemCategoryImages.Any())
        {
            ReadItemCategoryImages();
        }

        var escapedCategory = category.ToLower().Replace(" ", "-");
        if (escapedCategory.StartsWith(OneHandedName) || escapedCategory.StartsWith(TwoHandedName) ||
            WeaponsNames.Contains(escapedCategory))
        {
            escapedCategory = "weapon";
        }

        return ItemCategoryImages.ContainsKey(escapedCategory) ? ItemCategoryImages[escapedCategory] : string.Empty;
    }

    public static string ExtractItemName(string text)
    {
        if(string.IsNullOrEmpty(text) || !text.StartsWith("Item Class:")) return string.Empty;
        
        var lines = text.Split("\r\n");
        var i = 0;
        while (!lines[i].StartsWith("Rarity:"))
        {
            ++i;
        }

        ++i;
        
        var name = string.Empty;
        while (!lines[i].StartsWith("---"))
        {
            name += $"{lines[i]} ";
            ++i;
        }

        return name[..^1];
    }

    #endregion

    #region Private methods

    private static void ReadItemCategoryImages()
    {
        Log.Information("Loading item categories images...");
        foreach (var path in Directory.EnumerateFiles("./assets/chaosRecipe"))
        {
            ItemCategoryImages.Add(Path.GetFileNameWithoutExtension(path), Path.GetFullPath(path));
        }
    }

    private static void ReadItemCategories()
    {
        Log.Information("Loading item categories...");
        var data = File.ReadAllText("./data/item-categories.json");
        _itemCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(data) ?? new Dictionary<string, string>();
    }

    #endregion
}