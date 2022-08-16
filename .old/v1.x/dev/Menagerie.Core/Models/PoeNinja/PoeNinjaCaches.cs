using System;
using System.Collections.Generic;
using System.Text;
using Menagerie.Core.Models.PoeNinja;

namespace Menagerie.Core.Models
{
    public class PoeNinjaCaches : DbModel
    {
        public PoeNinjaCache<PoeNinjaCurrency> Currency { get; set; }
        public Dictionary<string, PoeNinjaCache<PoeNinjaItem>> Items { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}