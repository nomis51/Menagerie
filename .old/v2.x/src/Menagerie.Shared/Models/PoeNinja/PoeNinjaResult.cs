namespace Menagerie.Shared.Models.PoeNinja
{
    public class PoeNinjaResult<T>
    {
        public List<T> Lines { get; set; }
        public PoeNinjaResultLanguage Language { get; set; }
    }
}