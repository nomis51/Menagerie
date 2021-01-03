using Squirrel;
using System.Threading.Tasks;

namespace Menagerie.Services {
    public class UpdateService {

        public UpdateService() {}

        public async Task CheckUpdates() {
            using (var updateManager = await UpdateManager.GitHubUpdateManager("https://github.com/nomis51/Menagerie")) {
                await updateManager.UpdateApp();
                UpdateManager.RestartApp();
            }
        }
    }
}
