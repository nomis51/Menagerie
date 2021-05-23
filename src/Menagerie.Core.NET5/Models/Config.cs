using System.Collections.Generic;
using System.Drawing;

namespace Menagerie.Core.Models
{
    public class Config : DbModel
    {
        public string PlayerName { get; set; }
        public string CurrentLeague { get; set; }
        public bool OnlyShowOffersOfCurrentLeague { get; set; }
        public bool FilterSoldOffers { get; set; }
        public string SoldWhisper { get; set; }
        public string StillInterestedWhisper { get; set; }
        public string BusyWhisper { get; set; }
        public string ThanksWhisper { get; set; }
        public bool AutoThanks { get; set; }
        public bool AutoWhisper { get; set; }
        public bool AutoWhisperOutOfLeague { get; set; }
        public string OutOfLeagueWhisper { get; set; }
        public int PoeNinjaUpdateRate { get; set; }
        public List<string> ChatScanWords { get; set; } = new List<string>();
        public int ChaosRecipeTabIndex { get; set; }
        public int ChaosRecipeRefreshRate { get; set; }
        public int ChaosRecipeMaxSets { get; set; }
        public string POESESSID { get; set; }
        public bool ChaosRecipeEnabled { get; set; }
        public Point IncomingOffersGridOffset { get; set; }
        public Point IncomingOffersControlsGridOffset { get; set; }
        public Point OutgoingOffersGridOffset { get; set; }
        public Point ChaosRecipeGridOffset { get; set; }
        public bool ChaosRecipeOveralyDockMode { get; set; }
    }
}