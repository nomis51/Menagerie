using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class TradeRequestQueryStatFilter
    {
        public string Id { get; set; }
        public StatFilterValue Value { get; set; }
        public bool Disabled { get; set; }
    }
}