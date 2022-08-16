using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;

namespace Menagerie.Models
{
    public class MapModifier
    {
        public Regex Regex { get; set; }
        public bool IsGood { get; set; }
        public bool IsBad { get; set; }
        public IEnumerable<string> Keywords { get; set; }

        public string Description => string.Join(", ", Keywords);
        public Brush Color => IsBad ? Brushes.Red : IsGood ? Brushes.LimeGreen : Brushes.White;
    }
}