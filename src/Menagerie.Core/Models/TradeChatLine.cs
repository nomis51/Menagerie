using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeChatLine {
        public DateTime Time { get; set; }
        public string PlayerName { get; set; }
        public List<TradeChatWords> Words{ get; set; }
    }
}
