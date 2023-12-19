namespace Menagerie.Shared.Models.Setting;

public class IncomingTradesSettings
{
    public string BusyWhisper { get; set; }
    public string SoldWhisper { get; set; }
    public string StillInterestedWhisper { get; set; }
    public string ThanksWhisper { get; set; }
    public string InviteWhisper { get; set; }
    public bool AutoThanks { get; set; }
    public bool AutoKick { get; set; }
    public bool IgnoreOutOfLeague { get; set; }
    public bool IgnoreSoldItems { get; set; }
    public bool VerifyPrice { get; set; }
    public bool HighlightWithGrid { get; set; }
}