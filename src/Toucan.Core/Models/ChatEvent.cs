using System;
using System.Collections.Generic;
using System.Text;

namespace Toucan.Core.Models {
    public class ChatEvent {
        public Core.ChatEvent EvenType { get; set; } = Core.ChatEvent.Offer;
    }
}
