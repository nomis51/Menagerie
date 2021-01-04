using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class AppImage:DbModel {
        public string Link { get; set; }
        public string Base64 { get; set; }
    }
}
