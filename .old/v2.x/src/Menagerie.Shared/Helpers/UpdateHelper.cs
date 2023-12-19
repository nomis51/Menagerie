using Serilog;
using Squirrel;

namespace Menagerie.Shared.Helpers;

public static class UpdateHelper
{
    public static Task UpdateApp()
    {
        return Task.Run(async () =>
        {
            if (Environment.GetEnvironmentVariable("ENV") == "PoE" || Environment.GetEnvironmentVariable("ENV") == "dev") return;

            try
            {
                using var updateManager = await UpdateManager.GitHubUpdateManager("https://github.com/nomis51/Menagerie");
                var infos = await updateManager.CheckForUpdate();

                if (!infos.ReleasesToApply.Any()) return;

                Log.Information("New update available {version}. Installing...", infos.ReleasesToApply.First().Version);
                await updateManager.UpdateApp();
                Log.Information("Version {version} installed", infos.ReleasesToApply.First().Version);
            }
            catch (Exception e)
            {
                Log.Warning("Unable to check for updates / update the app: {Message}", e.Message);
            }
        });
    }
}