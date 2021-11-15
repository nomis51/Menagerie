using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class ItemProps {
        public int GemLevel { get; set; }
        public int Armour { get; set; }
        public int Evasion { get; set; }
        public int EnergyShield { get; set; }
        public int BlockChance { get; set; }
        public double CritChance { get; set; }
        public double AttackSpeed { get; set; }
        public List<int> PhysicalDamage { get; set; } = new List<int>();
        public double ElementalDamage { get; set; }
        public bool BlightedMap { get; set; } = false;
        public int MapTier { get; set; }
        public int AreaLevel { get; set; }
    }
}
