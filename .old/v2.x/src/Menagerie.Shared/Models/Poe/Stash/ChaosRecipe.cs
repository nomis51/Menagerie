namespace Menagerie.Shared.Models.Poe.Stash;

public class ChaosRecipe
{
    public int NbGloves { get; set; }
    public int NbBoots { get; set; }
    public int NbHelmets { get; set; }
    public int NbBodyArmours { get; set; }
    public int NbBelts { get; set; }
    public int NbWeapons { get; set; }
    public int NbShields { get; set; }
    public int Nb2HWeapons { get; set; }
    public int NbRings { get; set; }
    public int NbAmulets { get; set; }

    public int NbRingSets => (int)Math.Floor(NbRings / 2.0d);

    public int NbWeaponSets =>
        Nb2HWeapons +
        (int)Math.Max(
            Math.Floor(NbWeapons / 2.0d),
            Math.Min(NbWeapons, (double)NbShields)
        );

    public int NbSets => new[]
        {
            NbGloves,
            NbBoots,
            NbHelmets,
            NbBelts,
            NbBodyArmours,
            NbRingSets,
            NbAmulets,
            NbWeaponSets
        }
        .Min();
}