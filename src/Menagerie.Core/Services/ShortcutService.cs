using log4net;
using Menagerie.Core.Abstractions;
using Menagerie.Core.Models;
using System.Collections.Generic;
using System.Linq;
using Winook;
using Menagerie.Core.Extensions;

namespace Menagerie.Core.Services
{
    public class ShortcutService : IService
    {
        #region Constants

        private static readonly ILog Log = LogManager.GetLogger(typeof(ShortcutService));

        #endregion

        #region Members

        private readonly List<Shortcut> _shortcuts = new();

        #endregion

        public ShortcutService()
        {
            Log.Trace("Initializing ShortcutService");
        }

        public void RegisterShortcut(Shortcut shortcut)
        {
            Log.Trace($"Registering shortcut {shortcut}");

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
                Log.Trace($"Executing shortcut {s}");
                s.Action();
            }
        }

        public void Start()
        {
            Log.Trace("Starting ShortcutService");
        }
    }
}