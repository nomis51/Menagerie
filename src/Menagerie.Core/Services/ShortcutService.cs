using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput.Native;
using Winook;

namespace Menagerie.Core.Services {
    public class ShortcutService : IService {
        #region Members
        List<Shortcut> Shortcuts = new List<Shortcut>();
        #endregion

        public ShortcutService() { }

        public void RegisterShortcut(Shortcut shortcut) {
            Shortcuts.Add(shortcut);
        }

        public void HandleShortcut(KeyboardMessageEventArgs evt) {
            foreach (var s in Shortcuts) {
                if ((ushort)s.Key == evt.KeyValue && s.Alt == evt.Alt && s.Control == evt.Control && s.Shift == evt.Shift && (s.Direction == KeyDirection.Any || s.Direction == evt.Direction)) {
                    s.Action();
                }
            }
        }

        public void Start() {
        }
    }
}
