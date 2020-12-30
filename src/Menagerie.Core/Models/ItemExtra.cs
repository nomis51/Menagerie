using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
   public class ItemExtra {
        public ItemAltQuality AltQuality { get; set; } = ItemAltQuality.None;
        public ItemVeiled Veiled { get; set; } = ItemVeiled.None;
        public ProphecyMaster ProphecyMaster { get; set; } = ProphecyMaster.None;
    }
}
