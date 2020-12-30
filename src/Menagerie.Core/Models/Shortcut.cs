using System;
using System.Collections.Generic;
using System.Text;
using WindowsInput.Native;
using Winook;

namespace Menagerie.Core.Models {
    public class Shortcut {
        public bool Control { get; set; } = false;
        public bool Alt { get; set; } = false;
        public bool Shift { get; set; } = false;
        public KeyDirection Direction { get; set; } = KeyDirection.Any;
        public VirtualKeyCode Key { get; set; }
        public Action Action { get; set; }
    }
}
