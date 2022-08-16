using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using Menagerie.Shared.Helpers;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class ChaosRecipeContainerViewModel : ReactiveObject
{
    #region Members

    private readonly SourceList<ChaosRecipeViewModel> _chaosRecipeItems = new();

    #endregion

    #region Props

    public ReadOnlyObservableCollection<ChaosRecipeViewModel> ChaosRecipeItems;

    #endregion

    #region Constructors

    public ChaosRecipeContainerViewModel()
    {
        _chaosRecipeItems
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out ChaosRecipeItems)
            .Subscribe();

        _chaosRecipeItems.AddRange(new[]
        {
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Helmets", 0, ItemHelper.GetItemCategoryImageLink("helmet"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Boots", 0, ItemHelper.GetItemCategoryImageLink("boots"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Gloves", 0, ItemHelper.GetItemCategoryImageLink("gloves"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Belts", 0, ItemHelper.GetItemCategoryImageLink("belt"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Body Armours", 0, ItemHelper.GetItemCategoryImageLink("body armour"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Rings", 0, ItemHelper.GetItemCategoryImageLink("ring"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Amulets", 0, ItemHelper.GetItemCategoryImageLink("amulet"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Weapons", 0, ItemHelper.GetItemCategoryImageLink("weapon"))),
            new ChaosRecipeViewModel(new ChaosRecipeItemDto("Sets", 0, string.Empty))
        });

        AppEvents.OnNewChaosRecipe += AppEvents_OnNewChaosRecipe;
    }

    #endregion

    #region Public methods

    #endregion

    #region Private methods

    private void AppEvents_OnNewChaosRecipe(List<ChaosRecipeItemDto> items)
    {
        _chaosRecipeItems.Clear();

        foreach (var item in items)
        {
            _chaosRecipeItems.Add(new ChaosRecipeViewModel(item));
        }
    }

    #endregion
}