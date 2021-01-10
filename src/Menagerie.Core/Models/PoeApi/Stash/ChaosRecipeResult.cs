using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models.PoeApi.Stash {
    public class ChaosRecipeResult {
        public int NbGloves { get; set; } = 0;
        public int NbHelmets { get; set; } = 0;
        public int NbBoots { get; set; } = 0;
        public int NbBelts { get; set; } = 0;
        public int NbBodyArmours { get; set; } = 0;
        public int NbAmulets { get; set; } = 0;
        private int _nbRings = 0;
        public int NbRings {
            get {
                return (int)Math.Floor(_nbRings / 2.0f);
            }
            set {
                _nbRings = value;
            }
        }
        public int Nb2HWeapons { get; set; } = 0;
        private int _nb1HWeapons = 0;
        public int Nb1HWeapons {
            get {
                return Math.Max(_nb1HWeapons / 2, Math.Min(_nb1HWeapons, NbOffHands));
            }
            set {
                _nb1HWeapons = value;
            }
        }
        public int NbOffHands { get; set; } = 0;
        public int NbSets {
            get {
                return Math.Min(NbGloves, Math.Min(NbHelmets, Math.Min(NbBoots, Math.Min(NbBelts, Math.Min(NbBodyArmours, Math.Min(NbAmulets, Math.Min(NbRings, Math.Min(Nb1HWeapons, Nb2HWeapons))))))));
            }
        }

    }
}
