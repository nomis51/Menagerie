using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class FetchResultAccount {
        public string Name { get; set; }
        public string LastCharacterName { get; set; }
        public FetchResultAccountStatus Online { get; set; }
    }
}
