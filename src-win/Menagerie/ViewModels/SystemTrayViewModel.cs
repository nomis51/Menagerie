using System.Reflection;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class SystemTrayViewModel : ReactiveObject
{
    #region Props

    public string CurrenLeague => AppService.Instance.GetSettings().General.League;

    public string AppVersion
    {
        get
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version is null
                ? ""
                : $"{version.Major}.{version.Minor}.{version.Build} (Build {version.MinorRevision})";
        }
    }

    #endregion

    #region Public methods

    public void ExitApp()
    {
        AppService.Instance.PlayClickSoundEffect();
        System.Windows.Application.Current.Shutdown();
    }

    #endregion
}