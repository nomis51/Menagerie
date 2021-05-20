using System.Threading.Tasks;
using GitHubAutoUpdater;

namespace Menagerie.Services
{
    public class UpdateService
    {
        private readonly Updater _updater;

        public UpdateService()
        {
            _updater = new Updater("nomis51/Menagerie", "{app_name}-{tag_name}.zip");
        }

        public void CheckUpdates()
        {
            _updater.CheckUpdate();
        }
    }
}