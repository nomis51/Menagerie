using System.Collections.Generic;

namespace Menagerie.Models
{
    public class MapModifier
    {
        public bool IsGood { get; set; }
        public bool IsBad { get; set; }
        public IEnumerable<string> Keywords { get; set; }
    }
}