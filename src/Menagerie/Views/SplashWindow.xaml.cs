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

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window {
        private static readonly ILog log = LogManager.GetLogger(typeof(SplashWindow));

        public SplashViewModel vm;

        public SplashWindow() {
            InitializeComponent();

            log.Trace("Initializing SplashWindow");

            vm = new SplashViewModel();
            DataContext = vm;

            SetLogo();
        }

        private void SetLogo() {
            log.Trace("Setting logo");
            imgLogo.Source = ImageSourceFromBitmap(Properties.Resources.menagerie_logo_splash);
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource ImageSourceFromBitmap(Bitmap bmp) {
            log.Trace("Converting bitmap to imageSource");

            var handle = bmp.GetHbitmap();
            try {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            } finally { DeleteObject(handle); }
        }
    }
}
