using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class AppVersion {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public string AppFolder {
            get {
                return $"app-{ToString()}";
            }
        }

        public AppVersion(int major, int minor, int build) {
            Major = major;
            Minor = minor;
            Build = build;
        }

        public AppVersion() { }

        public override string ToString() {
            return $"{Major}.{Minor}.{Build}";
        }
    }
}
