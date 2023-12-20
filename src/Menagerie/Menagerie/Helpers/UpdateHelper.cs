using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Serilog;
using Squirrel;
using Squirrel.Sources;

namespace Menagerie.Helpers;

public class UpdateHelper
{
    #region Constants

    private const string UpdateUrl = "https://github.com/nomis51/menagerie";

    #endregion

    #region Public methods

    public static void HookSquirrel()
    {
        if (!IsInstalledApp()) return;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            SquirrelAwareApp.HandleEvents(
                onInitialInstall: OnAppInstall,
                onAppUninstall: OnAppUninstall,
                onEveryRun: OnAppRun
            );
        }
    }

    public static async Task<string> CheckForUpdates()
    {
        if (!IsInstalledApp()) return string.Empty;
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return string.Empty;

        try
        {
            Log.Information("Checking for updates");
            using var updateManager = new UpdateManager(new GithubSource(UpdateUrl, string.Empty, true));
            if (!updateManager.IsInstalledApp) return string.Empty;

            var info = await updateManager.CheckForUpdate();

            if (info.ReleasesToApply.Count == 0) return string.Empty;

            var version = info.ReleasesToApply.Last().Version.ToString();
            Log.Information("Downloading update {Version}", version);

            await updateManager.UpdateApp();
            Log.Information("Update {Version} downloaded", version);

            return version ?? string.Empty;
        }
        catch (Exception e)
        {
            Log.Warning("Failed to check / install updates: {Message}", e.Message);
        }

        return string.Empty;
    }

    #endregion

    #region Private methods

    private static bool IsInstalledApp()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;

        using var updateManager = new UpdateManager(new GithubSource(UpdateUrl, string.Empty, true));
        return updateManager.IsInstalledApp;
    }

    private static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            tools.CreateShortcutForThisExe();
        }
    }

    private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            tools.RemoveShortcutForThisExe();
        }
    }

    private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
    }

    #endregion
}