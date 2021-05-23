using System.Collections.Generic;

namespace Menagerie.Core.Models
{
    public abstract class PoeNinjaSparkLine
    {
        public double TotalChange { get; set; }
        public List<double?> Data { get; set; }
    }
}