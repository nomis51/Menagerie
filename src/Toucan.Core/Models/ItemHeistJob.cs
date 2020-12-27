using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class ItemHeistJob {
        public HeistJob Name { get; set; }
        public int Level { get; set; }
    }

    public enum HeistJob {
        Lockpicking,
        Counter_Thaumaturgy,
        Perception,
        Deception,
        Agility,
        Engineering,
        TrapDisarmament,
        Demolition,
        BruteForce
    }
}
