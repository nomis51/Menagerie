using System;
using System.Windows;
using Menagerie.Models;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class NavigationItemViewModel : ReactiveObject
{
    #region Members

    #endregion

    #region Props

    public NavigationItemConfig Config { get; }

    #endregion

    #region Constructors

    public NavigationItemViewModel(NavigationItemConfig config)
    {
        Config = config;
    }

    #endregion
}