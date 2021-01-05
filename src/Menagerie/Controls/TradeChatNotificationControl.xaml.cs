using AdonisUI.Controls;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Menagerie.Controls {
    /// <summary>
    /// Logique d'interaction pour TradeChatNotification.xaml
    /// </summary>
    public partial class TradeChatNotificationControl : UserControl {
        public string Title { get; set; }
        public List<TradeChatWords> ContentWords { get; set; }
        public string Player { get; set; }
        public string Time { get; set; }
        private Action CloseNotification;

        public TradeChatNotificationControl(TradeChatLine line, Action close) {
            InitializeComponent();

            Title = "Trade Chat Match";
            Player = $"{line.PlayerName}:";
            Time = $"[{line.Time.ToString("HH:mm")}]";
            ContentWords = line.Words;
            CloseNotification = close;

            DataContext = this;

            GenerateRuns();
        }

        private void GenerateRuns() {
            foreach(var w in ContentWords) {
                    Run txt = new Run();
                    txt.Text = w.Words;

                if (w.Highlighted) {
                    txt.Background = Brushes.Yellow;
                    txt.Foreground = Brushes.Black;
                } else {
                    txt.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#a0aec0"));
                }

                txtContent.Inlines.Add(txt);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            CloseNotification();
        }
    }
}
