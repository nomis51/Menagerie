using Menagerie.Application.DTOs.Settings;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class StashTabGridViewModel : ReactiveObject
{
    #region Members

    private bool _isVisible;

    #endregion

    #region Props

    public StashTabGridSettingsDto StashTabGridSettings { get; }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    #endregion

    #region Constructors

    public StashTabGridViewModel()
    {
        var settings = AppService.Instance.GetSettings();
        StashTabGridSettings = settings.StashTabGrid;
    }

    public void PlayClickSound()
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();
    }

    #endregion

    #region Public methods

    public void UpdatePosition(int x, int y)
    {
        StashTabGridSettings.X = x;
        StashTabGridSettings.Y = y;
        var settings = AppService.Instance.GetSettings();
        settings.StashTabGrid = StashTabGridSettings;
        AppService.Instance.SetSettings(settings);
    }

    public void UpdateSize(int width, int height)
    {
        StashTabGridSettings.Width = width;
        StashTabGridSettings.Height = height;
        var settings = AppService.Instance.GetSettings();
        settings.StashTabGrid = StashTabGridSettings;
        AppService.Instance.SetSettings(settings);
    }

    public void SaveStashTabGridType(string stashTab, int width, int height, bool hasFolderOffset)
    {
        AppService.Instance.SaveStashTabGridSettings(stashTab, width, height, hasFolderOffset);
    }

    #endregion
}