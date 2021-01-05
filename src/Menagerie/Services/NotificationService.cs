using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using Menagerie.Controls;
using Menagerie.Core.Models;

namespace Menagerie.Services {
    public class NotificationService {
        #region Singleton
        private static NotificationService _instance = new NotificationService();
        public static NotificationService Instance {
            get {
                if (_instance == null) {
                    _instance = new NotificationService();
                }

                return _instance;
            }
        }
        #endregion

        private const int DURATION = 8000;
        private bool Ready = false;
        private bool NotificationRunning = false;
        private TaskbarIcon TrayIcon;

        private NotificationService() {
        }

        public void Setup(TaskbarIcon trayIcon) {
            TrayIcon = trayIcon;
            Ready = true;
        }

        public void ShowTradeChatMatchNotification(TradeChatLine line) {
            if (Ready) {
                Task.Run(() => AudioService.Instance.PlayNotif2());

                if (!NotificationRunning) {
                    Task.Run(() => {
                        App.Current.Dispatcher.Invoke(delegate {
                            TrayIcon.ShowCustomBalloon(new TradeChatNotificationControl(line, TrayIcon.CloseBalloon), System.Windows.Controls.Primitives.PopupAnimation.Slide, DURATION);
                        });
                    });
                }
            }
        }
    }
}
