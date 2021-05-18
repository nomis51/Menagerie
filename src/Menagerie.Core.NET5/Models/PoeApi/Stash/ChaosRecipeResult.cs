using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models.PoeApi.Stash {
    public class ChaosRecipeResult {
        public int NbGloves { get; set; } = 0;
        public bool NeedGloves {
            get {
                return NbGloves < MaxSets;
            }
        }
        public int NbHelmets { get; set; } = 0;
        public bool NeedHelmets {
            get {
                return NbHelmets < MaxSets;
            }
        }
        public int NbBoots { get; set; } = 0;
        public bool NeedBoots {
            get {
                return NbBoots < MaxSets;
            }
        }
        public int NbBelts { get; set; } = 0;
        public bool NeedBelts {
            get {
                return NbBelts < MaxSets;
            }
        }
        public int NbBodyArmours { get; set; } = 0;
        public bool NeedBodyArmours {
            get {
                return NbBodyArmours < MaxSets;
            }
        }
        public int NbAmulets { get; set; } = 0;
        public bool NeedAmulets {
            get {
                return NbAmulets < MaxSets;
            }
        }
        public int NbRings { get; set; } = 0;
        public int NbRingSets {
            get {
                return (int)Math.Floor(NbRings / 2.0f);
            }
        }
        public bool NeedRings {
            get {
                return NbRingSets < MaxSets;
            }
        }
        public int Nb2HWeapons { get; set; } = 0;
        public int Nb1HWeapons { get; set; } = 0;
        public int NbOffHands { get; set; } = 0;
        public int NbWeaponSets {
            get {
                return Nb2HWeapons + Math.Max(Nb1HWeapons / 2, Math.Min(Nb1HWeapons, NbOffHands));
            }
        }
        public bool NeedWeapons {
            get {
                return NbWeaponSets < MaxSets;
            }
        }
        public int NbSets {
            get {
                return Math.Min(NbGloves, Math.Min(NbHelmets, Math.Min(NbBoots, Math.Min(NbBelts, Math.Min(NbBodyArmours, Math.Min(NbAmulets, Math.Min(NbRingSets, NbWeaponSets)))))));
            }
        }

        private int MaxSets;

        public ChaosRecipeResult(int maxSets = 3) {
            MaxSets = maxSets;
        }
    }
}
