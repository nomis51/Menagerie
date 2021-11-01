using System.Collections.Generic;

namespace Menagerie.Core.Models
{
    public class PoeNinjaResultLanguage
    {
        public string Name { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }
}