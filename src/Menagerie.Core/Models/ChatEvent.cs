using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class ChatEvent {
        public ChatEventEnum EvenType { get; set; } = ChatEventEnum.Offer;
    }
}
