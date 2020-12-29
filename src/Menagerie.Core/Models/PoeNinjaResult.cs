using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class PoeNinjaResult<T> {
        public List<T> Lines { get; set; }
        public PoeNinjaResultLanguage Language { get; set; }
    }

    public class PoeNinjaResultLanguage {
        public string Name { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }
}
