using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class AreaChangedEvent : ChatEvent {
        public string Name { get; set; }
        public string Type { get; set; }

        public AreaChangedEvent() {
            EvenType = Enums.ChatEventEnum.AreaJoined;
        }
    }
}
