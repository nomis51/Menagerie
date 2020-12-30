using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window {
        public SplashViewModel vm;

        public SplashWindow() {
            InitializeComponent();

            vm = new SplashViewModel();
            DataContext = vm;

            SetLogo();
        }

        private void SetLogo() {
            imgLogo.Source = ImageSourceFromBitmap(Properties.Resources.menagerie_logo_splash);
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        private ImageSource ImageSourceFromBitmap(Bitmap bmp) {
            var handle = bmp.GetHbitmap();
            try {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            } finally { DeleteObject(handle); }
        }
    }
}
