using System;
using Menagerie.Application.DTOs;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class ChaosRecipeViewModel : ReactiveObject
{
    #region Members

    private readonly ChaosRecipeItemDto _chaosRecipeItem;

    #endregion

    #region Props

    public string Label => _chaosRecipeItem.Label;
    public int Count => _chaosRecipeItem.Count;

    public bool HasIconLink => !string.IsNullOrEmpty(_chaosRecipeItem.IconLink);

    public Uri IconLinkUri => string.IsNullOrEmpty(_chaosRecipeItem.IconLink)
        ? new Uri("", UriKind.Relative)
        : new Uri(_chaosRecipeItem.IconLink,
            UriKind.Absolute);

    #endregion

    #region Constructors

    public ChaosRecipeViewModel(ChaosRecipeItemDto chaosRecipeItem)
    {
        _chaosRecipeItem = chaosRecipeItem;
    }

    #endregion
}