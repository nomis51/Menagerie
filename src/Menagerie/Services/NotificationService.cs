using System.Threading;
using System.Windows.Controls;
using System.Windows;
using Label = System.Windows.Controls.Label;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

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
        private Label Container;
        private TextBlock Title;
        private TextBlock Content;

        private NotificationService() {

        }

        public void CloseNotification() {
            if (Ready) {
                Container.Visibility = Visibility.Hidden;
            }
        }

        public void Setup(Label container, TextBlock title, TextBlock content) {
            Container = container;
            Title = title;
            Content = content;
            Ready = true;
        }

        private void DisplayNotifications() {
            Task.Run(() => {
                NotificationRunning = true;

                while (Notifications.Count > 0) {
                    var notif = Notifications.Dequeue();
                    DisplayNotification(notif.Item1, notif.Item2);
                }

                NotificationRunning = false;
            });
        }

        private void DisplayNotification(string title, string content) {
            App.Current.Dispatcher.Invoke(delegate {
                Title.Text = title;
                Content.Text = content;
                Container.Visibility = Visibility.Visible;
            });

            Task.Run(() => {
                Thread.Sleep(5000);

                App.Current.Dispatcher.Invoke(delegate {
                    Container.Visibility = Visibility.Hidden;
                    NotificationRunning = false;
                });
            }).Wait();
        }

        public void ShowNotification(string title, string msg) {
            if (Ready ) {
                Notifications.Enqueue(new Tuple<string, string>(title, msg));

                if (!NotificationRunning) {
                    DisplayNotifications();
                }
            }
        }
    }
}
