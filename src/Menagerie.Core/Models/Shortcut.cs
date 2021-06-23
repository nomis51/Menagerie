using Desktop.Robot;
using System;
using System.Collections.Generic;
using System.Text;
using Winook;

namespace Menagerie.Core.Models
{
    public class Shortcut
    {
        public bool Control { get; set; } = false;
        public bool Alt { get; set; } = false;
        public bool Shift { get; set; } = false;
        public KeyDirection Direction { get; set; } = KeyDirection.Any;
        public Key Key { get; set; }
        public Action Action { get; set; }

        public override string ToString()
        {
            return
                $"{(Control ? "Ctrl + " : "")}{(Shift ? "Shift + " : "")}{(Alt ? "Alt + " : "")}{(int) Key} {(Direction switch { KeyDirection.Any => "Any direction", KeyDirection.Down => "Down", _ => "Up" })} {Action}";
        }
    }
}