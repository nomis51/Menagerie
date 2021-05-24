using System.Threading.Tasks;
using Squirrel;

namespace Menagerie.Services
{
    public static class UpdateService
    {
        public static async Task CheckUpdates()
        {
            using var updateManager = await UpdateManager.GitHubUpdateManager("https://github.com/nomis51/Menagerie");
            await updateManager.UpdateApp();
        }
    }
}