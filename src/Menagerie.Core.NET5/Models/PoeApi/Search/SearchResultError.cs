using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class SearchResultError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}