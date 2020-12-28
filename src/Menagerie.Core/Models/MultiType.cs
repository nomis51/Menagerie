using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public enum Kind {
        Int,
        String,
        DateTime,
        Char
    }

    public class MultiType {
        int intValue;
        string stringValue;
        DateTime dateTimeValue;
        char charValue;
        Kind kind;

        public object Value {
            get {
                switch (kind) {
                    case Kind.Int:
                        return intValue;
                    case Kind.String:
                        return stringValue;
                    case Kind.DateTime:
                        return dateTimeValue;
                    case Kind.Char:
                        return charValue;
                    default:
                        return null;
                }

            }
        }

        public override string ToString() {
            return Value.ToString();
        }

        // Implicit construction
        public static implicit operator MultiType(int i) {
            return new MultiType { intValue = i, kind = Kind.Int };
        }
        public static implicit operator MultiType(string s) {
            return new MultiType { stringValue = s, kind = Kind.String };
        }
        public static implicit operator MultiType(DateTime dt) {
            return new MultiType { dateTimeValue = dt, kind = Kind.DateTime };
        }
        public static implicit operator MultiType(char c) {
            return new MultiType { charValue = c, kind = Kind.Char };
        }

        // Implicit value reference
        public static implicit operator int(MultiType MultiType) {
            if (MultiType.kind != Kind.Int) // Optionally, you could validate the usage
            {
                throw new InvalidCastException("Trying to use a " + MultiType.kind + " as an int");
            }
            return MultiType.intValue;
        }
        public static implicit operator string(MultiType MultiType) {
            return MultiType.stringValue;
        }
        public static implicit operator DateTime(MultiType MultiType) {
            return MultiType.dateTimeValue;
        }
        public static implicit operator char(MultiType MultiType) {
            return MultiType.charValue;
        }
    }
}
