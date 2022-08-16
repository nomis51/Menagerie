using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class FetchResultElement
    {
        public string Id { get; set; }
        public FetchResultElementItem Item { get; set; }
        public FetchResultElementListing Listing { get; set; }
    }
}