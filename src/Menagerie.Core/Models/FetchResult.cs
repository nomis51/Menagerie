using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class FetchResult {
        public List<FetchResultElement> Result { get; set; }
    }
}