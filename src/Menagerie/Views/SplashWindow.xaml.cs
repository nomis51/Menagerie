using Menagerie.ViewModels;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Serilog;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour SplashWindow.xaml
    /// </summary>
    public partial class SplashView : Window
    {
        public SplashView()
        {
            InitializeComponent();

            Log.Information("Initializing SplashWindow");

            DataContext = new SplashViewModel();

            SetLogo();
        }

        private void SetLogo()
        {
            Log.Information("Setting logo");
            imgLogo.Source = ImageSourceFromBitmap(new Bitmap("Assets/menagerie-logo-splash.png"));
        }

        private static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            Log.Information("Converting bitmap to imageSource");

            var handle = bmp.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}