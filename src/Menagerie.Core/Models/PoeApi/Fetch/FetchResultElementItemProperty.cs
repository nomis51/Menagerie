using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class FetchResultElementItemProperty
    {
        public string Name { get; set; }
        public List<List<object>> Values { get; set; }
        public int Type { get; set; }
    }
}