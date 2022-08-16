namespace Menagerie.Models
{
    public class ChaosRecipeResult
    {
        public int NbGloves { get; set; } = 0;

        public bool NeedGloves { get; set; } = true;

        public int NbHelmets { get; set; } = 0;

        public bool NeedHelmets { get; set; } = true;

        public int NbBoots { get; set; } = 0;

        public bool NeedBoots { get; set; } = true;

        public int NbBelts { get; set; } = 0;

        public bool NeedBelts { get; set; } = true;

        public int NbBodyArmours { get; set; } = 0;

        public bool NeedBodyArmours { get; set; } = true;

        public int NbAmulets { get; set; } = 0;

        public bool NeedAmulets { get; set; } = true;

        public int NbRings { get; set; } = 0;

        public int NbRingSets { get; set; }

        public bool NeedRings { get; set; } = true;

        public int Nb2HWeapons { get; set; } = 0;
        public int Nb1HWeapons { get; set; } = 0;
        public int NbOffHands { get; set; } = 0;

        public int NbWeaponSets { get; set; }

        public bool NeedWeapons { get; set; } = true;

        public int NbSets { get; set; }
    }
}