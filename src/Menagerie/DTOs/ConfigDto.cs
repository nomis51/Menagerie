using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menagerie.Models;

namespace Menagerie.DTOs {
    public class ConfigDto {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string CurrentLeague { get; set; }
        public bool OnlyShowOffersOfCurrentLeague { get; set; }

        public ConfigDto() { }

        public ConfigDto(Config config) {
            Id = config.Id;
            PlayerName = config.PlayerName;
            CurrentLeague = config.CurrentLeague;
            OnlyShowOffersOfCurrentLeague = config.OnlyShowOffersOfCurrentLeague;
        }
    }
}
