using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class FetchResult {
        public List<FetchResultElement> Result { get; set; }

    }

    public class FetchResultElement {
        public string Id { get; set; }
        public FetchResultElementItem Item { get; set; }
        public FetchResultElementListing Listing { get; set; }
    }

    public class FetchResultElementItem {
        [JsonProperty("ilvl")]
        public int ILvl { get; set; }
        public int StackSize { get; set; }
        public bool Corrupted { get; set; }
        public string Icon { get; set; }
        public List<FetchResultElementItemProperty> Properties { get; set; }

    }

    public class FetchResultElementItemProperty {
        public string Name { get; set; }
        public List<List<object>> Values { get; set; }
        public int Type { get; set; }
    }

    public class FetchResultElementListing {
        public string Indexed { get; set; }
        public FetchResultElementListingPrice Price { get; set; }
        public FetchResultAccount Account { get; set; }
        public double ChaosValue { get; set; }
    }

    public class FetchResultElementListingPrice {
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Type { get; set; }
    }

    public class FetchResultAccount {
        public string Name { get; set; }
        public string LastCharacterName { get; set; }
        public FetchResultAccountStatus Online { get; set; }
    }

    public class FetchResultAccountStatus {
        public string League { get; set; }
    }
}
