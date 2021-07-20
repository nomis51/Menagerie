using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menagerie.Models
{
    public class AiCurrencyAnalysis
    {
        public int StackSize { get; set; }
        public string StackSizeStr => $"{StackSize}x";
        public int StackSizeFontSize => StackSize <= 10 ? 16 : StackSize <= 100 ? 13 : 11;
        public string IconLink { get; set; }
    }
}
