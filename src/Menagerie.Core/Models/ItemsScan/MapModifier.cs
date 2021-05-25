using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Menagerie.Core.Models.ItemsScan
{
    public class MapModifier
    {
        public Regex Regex { get; set; }
        public bool IsGood { get; set; }
        public bool IsBad { get; set; }
        public IEnumerable<string> Keywords { get; set; }
    }
}