using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Winook;
using Serilog;

namespace Menagerie.Core.Services
{
    public class ShortcutService : IService
    {
        #region Members

        private readonly List<Shortcut> _shortcuts = new();

        #endregion

        public ShortcutService()
        {
            Log.Information("Initializing ShortcutService");
        }

        public void RegisterShortcut(Shortcut shortcut)
        {
            Log.Information($"Registering shortcut {shortcut}");

            if (!_shortcuts.Contains(shortcut))
            {
                _shortcuts.Add(shortcut);
            }
        }

        public void HandleShortcut(KeyboardMessageEventArgs evt)
        {
            foreach (var s in _shortcuts.Where(s =>
                (ushort) s.Key == evt.KeyValue && s.Alt == evt.Alt && s.Control == evt.Control &&
                s.Shift == evt.Shift && (s.Direction == KeyDirection.Any || s.Direction == evt.Direction)))
            {
                Log.Information($"Executing shortcut {s}");
                s.Action();
            }
        }

        public void Start()
        {
            Log.Information("Starting ShortcutService");
        }
    }
}