namespace Menagerie.Shared.Models.Setting;

public class Settings
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public AppSettings App { get; set; }
    public GeneralSettings General { get; set; }
    public IncomingTradesSettings IncomingTrades { get; set; }
    public OutgoingTradesSettings OutgoingTrades { get; set; }

    public Settings()
    {
        App = new AppSettings
        {
            PoeNinjaRefreshRate = 30,
        };
        General = new GeneralSettings
        {
            League = "Standard",
        };
        IncomingTrades = new IncomingTradesSettings
        {
            BusyWhisper = "I'm busy right now in {location}, I'll whisper you for the {item} when i'm ready",
            SoldWhisper = "I'm sorry, my {item} has already been sold",
            StillInterestedWhisper = "Are you still interested in my {item} listed for {price}?",
            InviteWhisper = "Your {item} for {price} is ready to be picked up",
            ThanksWhisper = "Thanks!",
            AutoKick = true,
            AutoThanks = true,
            IgnoreSoldItems = true,
            IgnoreOutOfLeague = true,
            HighlightWithGrid = true,
        };
        OutgoingTrades = new OutgoingTradesSettings
        {
            ThanksWhisper = "Thanks!",
            AutoLeave = true,
            AutoThanks = true
        };
    }
}