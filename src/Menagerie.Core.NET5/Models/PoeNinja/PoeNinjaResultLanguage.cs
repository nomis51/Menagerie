using System.Collections.Generic;

namespace Menagerie.Core.Models
{
    public abstract class PoeNinjaResultLanguage
    {
        public string Name { get; set; }
        public Dictionary<string, string> Translations { get; set; }
    }
}