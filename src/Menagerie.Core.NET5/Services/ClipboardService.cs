using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services
{
    public class ClipboardService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(ClipboardService));

        #endregion

        #region Members

        private string _lastText = "";
        private bool _firstTextSkipped;

        #endregion

        #region Constructors

        public ClipboardService()
        {
            Log.Trace("Initializing ClipboardService");
        }

        #endregion

        #region Private methods

        private async void Listen()
        {
            Log.Trace("Start listening for clipboard inputs");
            while (true)
            {
                await Task.Delay(500);

                var text = GetClipboard();

                if (!_firstTextSkipped)
                {
                    _firstTextSkipped = true;
                    _lastText = text;
                    continue;
                }

                if (string.IsNullOrEmpty(text) || text == _lastText) continue;
                Log.Trace("New clipboard input");
                _lastText = text;
                AppService.Instance.NewClipboardText(text);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        #endregion

        #region Public methods

        public static bool SetClipboard(string value)
        {
            Log.Trace("Setting clipboard value");
            var result = false;

            var t = new Thread(() =>
            {
                try
                {
                    TextCopy.ClipboardService.SetText(value);
                    result = true;
                }
                catch (Exception e)
                {
                    Log.Error("Error while settings clipboard value", e);
                }
            });
            t.Start();
            t.Join();

            return result;
        }

        #endregion

        public static string GetClipboard()
        {
            Log.Trace("Getting clipboard value");
            var text = "";

            var t = new Thread(delegate()
            {
                try
                {
                    text = TextCopy.ClipboardService.GetText();
                }
                catch (Exception e)
                {
                    Log.Error("Error while getting clipboard value", e);
                }
            });
            t.Start();
            t.Join();

            return text;
        }

        public void Start()
        {
            Log.Trace("Starting ClipboardService");
            Listen();
        }
    }
}