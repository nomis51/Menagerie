using System;
using System.Collections.Generic;
using System.Text;
using PoeLogsParser.Models;

namespace Menagerie.Core.Models
{
    public class TradeChatLine
    {
        public DateTime Time { get; private set; }
        public string PlayerName { get; private set; }
        public List<TradeChatWords> Words { get; set; }

        public TradeChatLine(ChatMessageLogEntry entry)
        {
            Time = entry.Time;
            PlayerName = entry.Player;
        }
    }
}