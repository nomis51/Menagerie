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

        private static object Lock = new();
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

        public bool SetClipboard(string value, int delay = 0)
        {
            Log.Trace("Setting clipboard value");

            lock (Lock)
            {
                try
                {
                    Thread.Sleep(delay);
                    TextCopy.ClipboardService.SetText(value);

                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("Error while settings clipboard value", e);
                }

                return false;
            }
        }


        public string GetClipboard(int delay = 0)
        {
            Log.Trace("Getting clipboard value");
            var text = "";

            try
            {
                Thread.Sleep(delay);
                text = TextCopy.ClipboardService.GetText();
            }
            catch (Exception e)
            {
                Log.Error("Error while getting clipboard value", e);
            }

            return text;
        }

        public void Start()
        {
            Log.Trace("Starting ClipboardService");
            Listen();
        }

        #endregion
    }
}