using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class TradeRequestType {
        public string Discriminator { get; set; }
        public string Option { get; set; }
    }
}
