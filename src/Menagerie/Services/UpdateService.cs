using System.Linq;
using System.Threading.Tasks;
using Squirrel;

namespace Menagerie.Services
{
    public static class UpdateService
    {
        public delegate void NewUpdateInstalledEvent();

        public static event NewUpdateInstalledEvent NewUpdateInstalled;

        public static void CheckUpdates()
        {
            Task.Run(async () =>
            {
                OnNewUpdateInstalled();
                
                using var updateManager =
                    await UpdateManager.GitHubUpdateManager("https://github.com/nomis51/Menagerie");
                var infos = await updateManager.CheckForUpdate();

                if (infos.ReleasesToApply.Any())
                {
                    var result = await updateManager.UpdateApp();
                    OnNewUpdateInstalled();
                }
            });
        }

        private static void OnNewUpdateInstalled()
        {
            NewUpdateInstalled?.Invoke();
        }
    }
}