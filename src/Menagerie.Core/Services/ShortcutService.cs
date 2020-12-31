using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput.Native;
using Winook;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services {
    public class ShortcutService : IService {
        #region Constants
        private static readonly ILog log = LogManager.GetLogger(typeof(ShortcutService));
        #endregion

        #region Members
        List<Shortcut> Shortcuts = new List<Shortcut>();
        #endregion

        public ShortcutService() {
            log.Trace("Initializing ShortcutService");
        }

        public void RegisterShortcut(Shortcut shortcut) {
            log.Trace($"Registering shortcut {shortcut}");
            Shortcuts.Add(shortcut);
        }

        public void HandleShortcut(KeyboardMessageEventArgs evt) {
            foreach (var s in Shortcuts) {
                if ((ushort)s.Key == evt.KeyValue && s.Alt == evt.Alt && s.Control == evt.Control && s.Shift == evt.Shift && (s.Direction == KeyDirection.Any || s.Direction == evt.Direction)) {
                    log.Trace($"Executing shortcut {s}");
                    s.Action();
                }
            }
        }

        public void Start() {
            log.Trace("Starting ShortcutService");
        }
    }
}
