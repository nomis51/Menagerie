using Menagerie.Core.Enums;

namespace Menagerie.Core.Models
{
    public class ChatEvent
    {
        public ChatEventEnum EvenType { get; set; } = ChatEventEnum.Offer;
    }
}