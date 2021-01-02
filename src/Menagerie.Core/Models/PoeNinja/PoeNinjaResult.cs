using System.Collections.Generic;

namespace Menagerie.Core.Models {
    public class PoeNinjaResult<T> {
        public List<T> Lines { get; set; }
        public PoeNinjaResultLanguage Language { get; set; }
    }
}
