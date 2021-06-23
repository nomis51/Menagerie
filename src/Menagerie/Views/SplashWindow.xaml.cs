using log4net;
using Menagerie.ViewModels;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Menagerie.Core.Extensions;

namespace Menagerie.Views
{
    /// <summary>
    /// Logique d'interaction pour SplashWindow.xaml
    /// </summary>
    public partial class SplashView : Window
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SplashView));

        public SplashView()
        {
            InitializeComponent();

            Log.Trace("Initializing SplashWindow");

            DataContext = new SplashViewModel();

            SetLogo();
        }

        private void SetLogo()
        {
            Log.Trace("Setting logo");
            imgLogo.Source = ImageSourceFromBitmap(new Bitmap("Assets/menagerie-logo-splash.png"));
        }

        private static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        {
            Log.Trace("Converting bitmap to imageSource");

            var handle = bmp.GetHbitmap();
            return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}