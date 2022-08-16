using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public class TradeRequest
    {
        public TradeRequestQuery Query { get; set; }
        public TradeRequestSort Sort { get; set; }
    }
}