using Menagerie.Shared.Models.Poe.Stash;

namespace Menagerie.Application.DTOs
{
    public class LogbookModifierDto : LogbookModifier
    {
        public new LogbookFactionDto Faction { get; set; }
    }
}
