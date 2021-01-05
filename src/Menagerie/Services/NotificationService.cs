using System.Threading;
using System.Windows.Controls;
using System.Windows;
using Label = System.Windows.Controls.Label;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Hardcodet.Wpf.TaskbarNotification;
using Menagerie.Controls;

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

        private Queue<Tuple<string, string>> Notifications = new Queue<Tuple<string, string>>();
        private bool Ready = false;
        private bool NotificationRunning = false;
        private TaskbarIcon TrayIcon;

        private NotificationService() {

        }

        public void Setup(TaskbarIcon trayIcon) {
            TrayIcon = trayIcon;
            Ready = true;
        }

        public void ShowTradeChatMatchNotification(string content) {
            Task.Run(() => {
                App.Current.Dispatcher.Invoke(delegate {
                    TrayIcon.ShowCustomBalloon(new TradeChatNotificationControl() { ContentText = content, Title = "New Trade Chat Match" }, System.Windows.Controls.Primitives.PopupAnimation.Slide, 5000);
                });
            });
        }
    }
}
