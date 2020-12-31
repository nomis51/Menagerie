using log4net;
using Menagerie.Core.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class ClipboardService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(ClipboardService));
        #endregion

        #region Members
        private string LastText = "";
        #endregion

        #region Constructors
        public ClipboardService() {
            log.Trace("Initializing ClipboardService");
        }
        #endregion

        #region Private methods
        private async void Listen() {
            log.Trace("Start listening for clipboard inputs");
            while (true) {
                await Task.Delay(500);

                string text = GetClipboard();

                if (!string.IsNullOrEmpty(text) && text != LastText) {
                    log.Trace("New clipboard input");
                    LastText = text;
                    AppService.Instance.NewClipboardText(text);
                }
            }
        }
        #endregion

        #region Public methods
        public bool SetClipboard(string value) {
            log.Trace("Setting clipboard value");
            bool result = false;

            Thread t = new Thread(() => {
                try {
                    Clipboard.SetDataObject(value, true, 10, 100);
                    result = true;
                } catch (Exception e) {
                    log.Error("Error while settings clipboard value", e);
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return result;
        }
        #endregion

        public string GetClipboard() {
            log.Trace("Getting clipboard value");
            string text = "";

            Thread t = new Thread(delegate () {
                try {
                    text = Clipboard.GetText();
                } catch (Exception e) {
                    log.Error("Error while getting clipboard value", e);
                }
            });
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return text;
        }

        public void Start() {
            log.Trace("Starting ClipboardService");
            Listen();
        }
    }
}
