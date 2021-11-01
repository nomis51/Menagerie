using System;
using System.Drawing;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Win32;

namespace Menagerie.Core.Services
{
    public class ScreenCaptureService : IService
    {
        #region Constructors

        public ScreenCaptureService()
        {
        }

        #endregion


        #region Public methods

        public Bitmap CaptureArea(Rectangle bounds)
        {
            var result = new Bitmap(bounds.Width, bounds.Height);

            using var graphics = Graphics.FromImage(result);
            graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);

            return result;
        }
        
        public Bitmap CaptureDesktop()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }

        public Bitmap CaptureActiveWindow()
        {
            return CaptureWindow(User32.GetForegroundWindow());
        }

        public Bitmap CaptureWindow(IntPtr handle)
        {
            var rect = new User32.Rect();
            User32.GetWindowRect(handle, ref rect);
            var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            var result = new Bitmap(bounds.Width, bounds.Height);

            using var graphics = Graphics.FromImage(result);
            graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);

            return result;
        }


        public void Start()
        {
            
        }

        #endregion
    }
}