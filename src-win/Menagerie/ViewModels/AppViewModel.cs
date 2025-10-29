using System;
using System.Windows;
using Menagerie.Application.Events;
using Menagerie.Application.Services;
using Menagerie.Extensions;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class AppViewModel : ReactiveObject
{
    #region Events

    public delegate void OverlayVisibilityChangeEvent(bool isVisible);

    public event OverlayVisibilityChangeEvent OnOverlayVisibilityChange;

    public delegate void StashGridVisibilityChangeEvent(bool isVisible, int width, int height, int left, int top, int leftSize = 1, int topSize = 1, string stashTab = "",
        bool hasFolderOffset = false);

    public event StashGridVisibilityChangeEvent OnStashGridVisibilityChange;

    public delegate void TranslatorVisibleEvent();

    public event TranslatorVisibleEvent OnTranslatorVisible;

    #endregion

    #region Members

    private SettingsWindow? _settingsWindow;
    private bool _isStatsVisible;
    private bool _isTranslationToolVisible;
    private bool _isChaosRecipeVisible;
    private bool _isBulkTradeVisible;

    #endregion

    #region Props

    public NavigationViewModel NavigationViewModel { get; set; }

    public bool IsChaosRecipeVisible
    {
        get => _isChaosRecipeVisible;
        private set => this.RaiseAndSetIfChanged(ref _isChaosRecipeVisible, value);
    }

    public bool IsBulkTradeVisible
    {
        get => _isBulkTradeVisible;
        private set => this.RaiseAndSetIfChanged(ref _isBulkTradeVisible, value);
    }

    public bool IsStatsVisible
    {
        get => _isStatsVisible;
        private set => this.RaiseAndSetIfChanged(ref _isStatsVisible, value);
    }

    public bool IsTranslationToolVisible
    {
        get => _isTranslationToolVisible;
        private set => this.RaiseAndSetIfChanged(ref _isTranslationToolVisible, value);
    }

    #endregion

    #region Constructors

    public AppViewModel()
    {
        NavigationViewModel = new NavigationViewModel();
        NavigationViewModel.OnToggleSettingsView += Navigation_OnToggleSettingsView;
        NavigationViewModel.OnToggleStatisticsView += Navigation_OnToggleStatisticsView;
        NavigationViewModel.OnToggleTranslatorView += Navigation_OnToggleTranslatorView;
        NavigationViewModel.OnToggleChaosRecipeView += Navigation_OnToggleChaosRecipeView;
        NavigationViewModel.OnToggleBulkTradeView += Navigation_OnToggleBulkTradeView;

        var settings = AppService.Instance.GetSettings();
        IsChaosRecipeVisible = settings.ChaosRecipe.Enabled;

        AppEvents.OnOverlayVisibilityChange += AppEvents_OnOverlayVisibilityChange;
        AppEvents.OnHighlightItem += AppEvents_OnHighlightItem;
    }

    #endregion

    #region Public methods

    public void ToggleStatistics()
    {
        IsStatsVisible = !IsStatsVisible;
    }

    public void ToggleBulkTrade()
    {
        IsBulkTradeVisible = !IsBulkTradeVisible;
    }

    public void ToggleTranslator()
    {
        IsTranslationToolVisible = !IsTranslationToolVisible;

        if (IsTranslationToolVisible)
        {
            AppService.Instance.EnsureOverlayFocused();
            OnTranslatorVisible?.Invoke();
        }
        else
        {
            AppService.Instance.EnsureGameFocused();
        }
    }

    public void SetOverlayHandle(IntPtr handle)
    {
        AppService.Instance.SetOverlayHandle(handle);
    }

    #endregion

    #region Private methods

    private void Navigation_OnToggleBulkTradeView()
    {
        ToggleBulkTrade();
    }

    private void ToggleSettings()
    {
        if (_settingsWindow is null || !_settingsWindow.IsLoaded)
        {
            var settings = AppService.Instance.GetSettings();
            var settingsViewModel = new SettingsViewModel(settings);
            _settingsWindow = settingsViewModel.GetWindowView<SettingsViewModel, SettingsWindow>();
            _settingsWindow.Show();
        }
        else if (_settingsWindow.WindowState == WindowState.Minimized)
        {
            _settingsWindow.WindowState = WindowState.Normal;
        }
        else
        {
            AppService.Instance.EnsureGameFocused();
            _settingsWindow.Close();
        }
    }

    private void ToggleChaosRecipe()
    {
        IsChaosRecipeVisible = !IsChaosRecipeVisible;
    }

    private void Navigation_OnToggleChaosRecipeView()
    {
        ToggleChaosRecipe();
    }

    private void Navigation_OnToggleTranslatorView()
    {
        ToggleTranslator();
    }

    private void Navigation_OnToggleStatisticsView()
    {
        ToggleStatistics();
    }

    private void Navigation_OnToggleSettingsView()
    {
        ToggleSettings();
    }

    private void AppEvents_OnHighlightItem(bool isVisible, int left, int top, int leftSize, int topSize, string stashTab)
    {
        var settings = AppService.Instance.GetSettings();
        var gridSettings = settings.StashTabGrid.TabsGridSettings.Find(t => t.StashTab == stashTab);

        OnStashGridVisibilityChange?.Invoke(isVisible, gridSettings?.Width ?? 12, gridSettings?.Height ?? 12, left, top, leftSize, topSize, stashTab,
            gridSettings?.HasFolderOffset ?? false);
    }


    private void AppEvents_OnOverlayVisibilityChange(bool isVisible)
    {
        OnOverlayVisibilityChange?.Invoke(isVisible);
    }

    #endregion
}