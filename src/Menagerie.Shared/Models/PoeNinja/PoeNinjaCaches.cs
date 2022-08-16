namespace Menagerie.Shared.Models.PoeNinja
{
    public class PoeNinjaCaches : Entity
    {
        public PoeNinjaCache<PoeNinjaCurrency> Currency { get; set; }
        public Dictionary<string, PoeNinjaCache<PoeNinjaItem>> Items { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}