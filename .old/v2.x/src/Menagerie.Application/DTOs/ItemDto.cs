using Menagerie.Shared.Models.Poe.Stash;

namespace Menagerie.Application.DTOs;

public class ItemDto : Item
{
    public new List<PropertyDto> Properties { get; set; } = new();
    public new List<RequirementDto> Requirements { get; set; } = new();
    public new List<SocketDto> Sockets { get; set; } = new();
    public new List<LogbookModifierDto> LogbookMods { get; set; } = new();

}
