using System;
using System.Threading.Tasks;
using Menagerie.Helpers;
using Menagerie.Shared.Helpers;
using ReactiveUI;
using Serilog;

namespace Menagerie.ViewModels;

public class AppViewModel : ViewModelBase
{
    #region Props

    private string _versionText = $"Version {VersionHelper.GetVersion()}";

    public string VersionText
    {
        get => _versionText;
        set => this.RaiseAndSetIfChanged(ref _versionText, value);
    }

    private bool _checkingForUpdates;

    #endregion

    #region Constructors

    public AppViewModel()
    {
        CheckForUpdates();
    }

    #endregion

    #region Public methods

    public void CheckForUpdates()
    {
        if (_checkingForUpdates) return;

        _checkingForUpdates = true;
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(5000);
                var version = await UpdateHelper.CheckForUpdates();
                if (string.IsNullOrEmpty(version)) return;

                VersionText += $" (Version {version} will be installed after a restart)";
            }
            catch (Exception e)
            {
                Log.Error("Failed to check for updates: {Message}", e.Message);
            }
            finally
            {
                _checkingForUpdates = false;
            }
        });
    }

    #endregion
}