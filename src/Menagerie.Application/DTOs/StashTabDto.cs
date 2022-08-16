using Menagerie.Shared.Models.Poe.Stash;

namespace Menagerie.Application.DTOs
{
    public class StashTabDto : StashTab
    {
        public new TabColorDto Color { get; set; }
        public new List<ItemDto> Items { get; set; } = new();
    }
}
