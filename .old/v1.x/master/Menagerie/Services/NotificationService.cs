using System.Threading.Tasks;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Menagerie.Controls;
using Menagerie.Core.Models;

namespace Menagerie.Services
{
    public class NotificationService
    {
        #region Singleton

        private static NotificationService _instance = new();

        public static NotificationService Instance => _instance ??= new NotificationService();

        #endregion

        private const int Duration = 8000;
        private bool _ready;
        private bool NotificationRunning = false;
        private TaskbarIcon _trayIcon;

        private NotificationService()
        {
        }

        public void Setup(TaskbarIcon trayIcon)
        {
            _trayIcon = trayIcon;
            _ready = true;
        }

        public void ShowNewUpdateInstalledNotification()
        {
            if (!_ready) return;
            Task.Run(() => AudioService.Instance.PlayNotification2());

            if (!NotificationRunning)
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        _trayIcon.ShowCustomBalloon(
                            new NewUpdateInstalled(_trayIcon.CloseBalloon),
                            System.Windows.Controls.Primitives.PopupAnimation.Slide, 60000);
                    });
                });
            }
        }

        public void ShowTradeChatMatchNotification(TradeChatLine line)
        {
            if (!_ready) return;
            Task.Run(() => AudioService.Instance.PlayNotification2());

            if (!NotificationRunning)
            {
                Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        _trayIcon.ShowCustomBalloon(
                            new TradeChatNotificationControl(line, _trayIcon.CloseBalloon),
                            System.Windows.Controls.Primitives.PopupAnimation.Slide, Duration);
                    });
                });
            }
        }
    }
}