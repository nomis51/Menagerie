namespace Menagerie.Shared.Models.Setting;

public class Settings
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public AppSettings App { get; set; }
    public GeneralSettings General { get; set; }
    public IncomingTradesSettings IncomingTrades { get; set; }
    public OutgoingTradesSettings OutgoingTrades { get; set; }
    public ChaosRecipeSettings ChaosRecipe { get; set; }
    public ChatScanSettings ChatScan { get; set; }
    public RecordingSettings Recording { get; set; }
    public DeathReplaySettings DeathReplay { get; set; }
    public ReplaySettings Replay { get; set; }
    public StashTabGridSettings StashTabGrid { get; set; }

    public Settings()
    {
        App = new AppSettings
        {
            PoeNinjaRefreshRate = 30,
        };
        General = new GeneralSettings
        {
            League = "Standard",
            Poesessid = "",
            AccountName = "",
            EnableSentry = true
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
        ChaosRecipe = new ChaosRecipeSettings
        {
            Enabled = true,
            StashTabIndex = 0,
            RefreshRate = 2,
        };
        ChatScan = new ChatScanSettings
        {
            Words = new List<string>(),
            AutoRemoveMessage = true,
            AutoRemoveMessageDelay = 10
        };
        Recording = new RecordingSettings
        {
            Enabled = true,
            FrameRate = 30,
            Crf = 25,
            ClipDuration = 5 * 60,
            CleanupTimeout = 30 * 60,
            ClipSaveDelay = 2,
            ClipSavePadding = 1,
            NbClipsToKeep = 5,
            OutputPath = "%USERPROFILE%/Documents/My Games/Menagerie/replays/",
            Threads = "auto"
        };
        DeathReplay = new DeathReplaySettings
        {
            OutputPath = "%USERPROFILE%/Documents/My Games/Menagerie/replays/deaths/",
            Duration = 10
        };
        Replay = new ReplaySettings
        {
            OutputPath = "%USERPROFILE%/Documents/My Games/Menagerie/replays/",
            Duration = 30
        };
        StashTabGrid = new StashTabGridSettings
        {
            X = 15,
            Y = -156,
            Width = 635,
            Height = 660,
            IncrementValue = 4,
            HighlightBorderThickness = 2,
            DropShadow = true,
            DropShadowOpacity = 0.5,
            FolderOffset = 70,
        };
    }
}