using System;
using System.Windows;
using System.Windows.Controls;

namespace Menagerie.Controls
{
    public partial class NewUpdateInstalled : UserControl
    {
        private readonly Action _close;

        public NewUpdateInstalled(Action close)
        {
            InitializeComponent();
            _close = close;
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            _close();
        }

        private void BtnRestart_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}