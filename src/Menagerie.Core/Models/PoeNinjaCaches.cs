using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class PoeNinjaCaches {
        public PoeNinjaCache<PoeNinjaCurrency> Currency { get; set; }
        public Dictionary<string, PoeNinjaCache<PoeNinjaItem>> Items { get; set; }
    }
}
