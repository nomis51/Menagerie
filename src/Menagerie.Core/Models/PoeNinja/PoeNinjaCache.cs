using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class PoeNinjaCache<T> {
        public Dictionary<string, List<T>> Map { get; set; }
        public PoeNinjaResultLanguage Language { get; set; }
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}