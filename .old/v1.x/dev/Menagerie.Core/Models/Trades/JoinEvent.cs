using Menagerie.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public class JoinEvent : ChatEvent
    {
        public string PlayerName { get; set; }

        public JoinEvent(string playerName)
        {
            EvenType = ChatEventEnum.PlayerJoined;
            PlayerName = playerName;
        }
    }
}