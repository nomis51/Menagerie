using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
   public class ItemExtra {
        public ItemAltQuality AltQuality { get; set; } = ItemAltQuality.None;
        public ItemVeiled Veiled { get; set; } = ItemVeiled.None;
        public ProphecyMaster ProphecyMaster { get; set; } = ProphecyMaster.None;
    }

    public enum ItemVeiled {
        Prefix,
        Suffix,
        PrefixAndSuffix,
        None
    }

    public enum ItemAltQuality {
        Anomalous,
        Divergent,
        Phantasmal,
        Superior,
        None
    }

    public enum ProphecyMaster {
        Alva,
        Einhar,
        Niko,
        Jun,
        Zana,
        None
    }
}
