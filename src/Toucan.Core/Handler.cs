using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core {
    public class Handler {
        public bool Ready { get; protected set; }

        public virtual void Start() { }
    }
}
