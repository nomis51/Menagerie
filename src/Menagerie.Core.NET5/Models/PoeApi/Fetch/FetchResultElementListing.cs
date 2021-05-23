using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class FetchResultElementListing
    {
        public string Indexed { get; set; }
        public FetchResultElementListingPrice Price { get; set; }
        public FetchResultAccount Account { get; set; }
        public double ChaosValue { get; set; }
    }
}