using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Media;
using DynamicData;
using FontAwesome5;
using Menagerie.Application.Services;
using Menagerie.Models;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class NavigationViewModel : ReactiveObject
{
    #region Events

    public delegate void ToggleStatisticsViewEvent();

    public event ToggleStatisticsViewEvent OnToggleStatisticsView;

    public delegate void ToggleSettingsViewEvent();

    public event ToggleSettingsViewEvent OnToggleSettingsView;

    public delegate void ToggleChaosRecipeViewEvent();

    public event ToggleChaosRecipeViewEvent OnToggleChaosRecipeView;

    public delegate void ToggleTranslatorViewEvent();

    public event ToggleTranslatorViewEvent OnToggleTranslatorView;

    #endregion

    #region Constants

    private const int ButtonWidth = 40;
    private const int ButtonHeight = 40;
    private readonly Thickness _buttonMargin = new(5);
    private const int IconWidth = 20;
    private const int IconHeight = 20;
    private readonly Thickness _iconMargin = new(5);

    #endregion

    #region Members

    private readonly SourceList<NavigationItemViewModel> _navigationItems = new();
    private bool _areNavigationItemsVisible;

    #endregion

    #region Props

    public ReadOnlyObservableCollection<NavigationItemViewModel> NavigationItems;

    public bool AreNavigationItemsVisible
    {
        get => _areNavigationItemsVisible;
        private set => this.RaiseAndSetIfChanged(ref _areNavigationItemsVisible, value);
    }

    #endregion

    #region Constructors

    public NavigationViewModel()
    {
        _navigationItems
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out NavigationItems)
            .Subscribe();

        Initialize();
    }

    #endregion

    #region Public methods

    public void ToggleToolsButtons()
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();
        AreNavigationItemsVisible = !AreNavigationItemsVisible;
    }

    #endregion

    #region Private methods

    private void Initialize()
    {
        if (System.Windows.Application.Current.MainWindow is null) throw new NullReferenceException("MainWindow was null");
        var materialDesignFloatingButtonStyle = (Style)System.Windows.Application.Current.MainWindow.FindResource("MaterialDesignFloatingActionLightButton");
        var red = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Red");
        var yellow = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Yellow");
        var green = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Green");
        var purple = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Purple");
        var orange = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Orange");
        var blue = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Blue");
        var primary = (SolidColorBrush)System.Windows.Application.Current.MainWindow.FindResource("Primary");

        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = red,
            BorderBrush = red,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonQuit_Click,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_SignOutAlt,
                Margin = _iconMargin
            }
        }));
        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = yellow,
            BorderBrush = yellow,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonSaveClip_Click,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_Film,
                Margin = _iconMargin
            }
        }));
        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = green,
            BorderBrush = green,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonTranslate_OnClick,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_Language,
                Margin = _iconMargin
            }
        }));
        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = purple,
            BorderBrush = purple,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonStats_OnClick,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_ChartLine,
                Margin = _iconMargin
            }
        }));
        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = orange,
            BorderBrush = orange,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonChaosRecipe_OnClick,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_DollarSign,
                Margin = _iconMargin
            }
        }));
        _navigationItems.Add(new NavigationItemViewModel(new NavigationItemConfig
        {
            Width = ButtonWidth,
            Height = ButtonHeight,
            Margin = _buttonMargin,
            Background = blue,
            BorderBrush = blue,
            Style = materialDesignFloatingButtonStyle,
            OnClickFn = ButtonSettings_OnClick,
            IconConfig = new NavigationItemIconConfig
            {
                Width = IconWidth,
                Height = IconHeight,
                Foreground = primary,
                Icon = EFontAwesomeIcon.Solid_Cog,
                Margin = _iconMargin
            }
        }));
    }

    private void ButtonSaveClip_Click(object sender, RoutedEventArgs e)
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.EnsureGameFocused();
        
        ToggleToolsButtons();
        
        AppService.Instance.SaveLastClip();
    }

    private void ButtonQuit_Click(object? sender, RoutedEventArgs e)
    {
        ExitApp();
    }

    private void ButtonSettings_OnClick(object sender, RoutedEventArgs e)
    {
        AppService.Instance.PlayClickSoundEffect();

        ToggleSettings();
        ToggleToolsButtons();
    }

    private void ButtonChaosRecipe_OnClick(object sender, RoutedEventArgs e)
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.EnsureGameFocused();

        ToggleChaosRecipe();
        ToggleToolsButtons();
    }


    private void ButtonTranslate_OnClick(object sender, RoutedEventArgs e)
    {
        AppService.Instance.PlayClickSoundEffect();

        ToggleToolsButtons();
        ToggleTranslationTool();
    }

    private void ButtonStats_OnClick(object sender, RoutedEventArgs e)
    {
        AppService.Instance.PlayClickSoundEffect();

        ToggleStats();
        ToggleToolsButtons();
    }

    private void ExitApp()
    {
        AppService.Instance.PlayClickSoundEffect();
        ToggleToolsButtons();
        System.Windows.Application.Current.Shutdown();
    }

    private void ToggleStats()
    {
        OnToggleStatisticsView?.Invoke();
    }

    private void ToggleSettings()
    {
        OnToggleSettingsView?.Invoke();
    }

    private void ToggleChaosRecipe()
    {
        OnToggleChaosRecipeView?.Invoke();
    }

    private void ToggleTranslationTool()
    {
        OnToggleTranslatorView?.Invoke();
    }

    #endregion
}