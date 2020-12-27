using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class ItemSocket {
        public SocketColor Color { get; set; }
        public bool LinkedToNext { get; set; } = false;
        public bool LinkedToPrevious { get; set; } = false;
    }

    public enum SocketColor {
        Red,
        Green,
        Blue,
        White
    }
}
