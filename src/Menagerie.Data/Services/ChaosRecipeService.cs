using Menagerie.Shared.Abstractions;
using Menagerie.Shared.Helpers;
using Menagerie.Shared.Models.Poe;
using Menagerie.Shared.Models.Poe.Stash;

namespace Menagerie.Data.Services;

public class ChaosRecipeService : IService
{
    #region Public methods

    public void Initialize()
    {
    }

    public Task Start()
    {
        AutoFetchChaosRecipe();
        return Task.CompletedTask;
    }

    #endregion

    #region Private methods

    private async Task<ChaosRecipe?> DoChaosRecipe()
    {
        var settings = AppDataService.Instance.GetSettings();
        var stashTab = AppDataService.Instance.GetStashTab(settings.ChaosRecipe.StashTabIndex);
        return stashTab is null ? null : CalculateChaosRecipe(stashTab);
    }

    private void AutoFetchChaosRecipe()
    {
        Task.Run(async () =>
        {
            Thread.Sleep(5000);

            while (true)
            {
                var settings = AppDataService.Instance.GetSettings();
                if (settings.ChaosRecipe.Enabled && !string.IsNullOrEmpty(settings.General.Poesessid) && !string.IsNullOrEmpty(settings.General.AccountName))
                {
                    var chaosRecipe = await DoChaosRecipe();

                    if (chaosRecipe is not null)
                    {
                        AppDataService.Instance.NewChaosRecipe(chaosRecipe);
                    }
                }

                Thread.Sleep(settings.ChaosRecipe.RefreshRate * 60 * 1000);
            }
        });
    }

    private ChaosRecipe? CalculateChaosRecipe(StashTab stashTab)
    {
        ChaosRecipe? chaosRecipe = new();

        foreach (var category in from item in stashTab.Items
                 where item.FrameType == 2 && (item.ItemLevel is >= 60 and < 75)
                 select ItemHelper.GetItemCategory(item.Type)
                 into category
                 where !string.IsNullOrEmpty(category)
                 select category)
        {
            switch (category)
            {
                case "Body Armour":
                    ++chaosRecipe.NbBodyArmours;
                    break;

                case "Helmet":
                    ++chaosRecipe.NbHelmets;
                    break;

                case "Boots":
                    ++chaosRecipe.NbBoots;
                    break;

                case "Gloves":
                    ++chaosRecipe.NbGloves;
                    break;

                case "Belt":
                    ++chaosRecipe.NbBelts;
                    break;

                case "Ring":
                    ++chaosRecipe.NbRings;
                    break;

                case "Amulet":
                    ++chaosRecipe.NbAmulets;
                    break;

                case "Wand":
                case "Scepter":
                case "Dagger":
                case "Claw":
                case "Rune Dagger":
                case { } s when s.StartsWith("One-Handed"):
                    ++chaosRecipe.NbWeapons;
                    break;

                case "Shield":
                    ++chaosRecipe.NbShields;
                    break;

                case "Bow":
                case "Staff":
                case "Warstaff":
                case { } s when s.StartsWith("Two-Handed"):
                    ++chaosRecipe.Nb2HWeapons;
                    break;
            }
        }

        return chaosRecipe;
    }

    #endregion
}