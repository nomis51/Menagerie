using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DynamicData;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class SettingsViewModel : ReactiveObject
{
    #region Members

    private string _selectedNavigationItem;
    private SettingsDto _settings;
    private readonly SourceList<string> _leagues = new();
    private bool _loading = true;

    #endregion

    #region Props

    public ReadOnlyObservableCollection<string> NavigationItems = new(new ObservableCollection<string>
    {
        "General",
        "Incoming trades",
        "Outgoing trades",
        "Chaos recipe",
        "Chat scan",
    });

    public string SelectedNavigationItem
    {
        get => _selectedNavigationItem;
        set => this.RaiseAndSetIfChanged(ref _selectedNavigationItem, value);
    }

    public string AppVersion
    {
        get
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version is null
                ? ""
                : $"Application Version: {version.Major}.{version.Minor}.{version.Build} (Build {version.MinorRevision})";
        }
    }

    public ReadOnlyObservableCollection<string> Leagues;

    /**
   * General
   */
    public string Poesessid
    {
        get => _settings.General.Poesessid;
        set
        {
            _settings.General.Poesessid = value;
            SaveSettings();
        }
    }

    public string AccountName
    {
        get => _settings.General.AccountName;
        set
        {
            _settings.General.AccountName = value;
            SaveSettings();
        }
    }

    public string League
    {
        get => _settings.General.League;
        set
        {
            _settings.General.League = value;
            SaveSettings();
        }
    }
    
    public bool EnableRecording
    {
        get => _settings.Recording.Enabled;
        set
        {
            _settings.Recording.Enabled = value;
            SaveSettings();
        }
    }
    
    /**
     * Incoming trades
     */
    public string BusyWhisperIncomingTrades
    {
        get => _settings.IncomingTrades.BusyWhisper;
        set
        {
            _settings.IncomingTrades.BusyWhisper = value;
            SaveSettings();
        }
    }

    public string SoldWhisperIncomingTrades
    {
        get => _settings.IncomingTrades.SoldWhisper;
        set
        {
            _settings.IncomingTrades.SoldWhisper = value;
            SaveSettings();
        }
    }

    public string StillInterestedWhisperIncomingTrades
    {
        get => _settings.IncomingTrades.StillInterestedWhisper;
        set
        {
            _settings.IncomingTrades.StillInterestedWhisper = value;
            SaveSettings();
        }
    }

    public string ThanksWhisperIncomingTrades
    {
        get => _settings.IncomingTrades.ThanksWhisper;
        set
        {
            _settings.IncomingTrades.ThanksWhisper = value;
            SaveSettings();
        }
    }

    public string InviteWhisperIncomingTrades
    {
        get => _settings.IncomingTrades.InviteWhisper;
        set
        {
            _settings.IncomingTrades.InviteWhisper = value;
            SaveSettings();
        }
    }

    public bool AutoThanksIncomingTrades
    {
        get => _settings.IncomingTrades.AutoThanks;
        set
        {
            _settings.IncomingTrades.AutoThanks = value;
            SaveSettings();
        }
    }

    public bool AutoKickIncomingTrades
    {
        get => _settings.IncomingTrades.AutoKick;
        set
        {
            _settings.IncomingTrades.AutoKick = value;
            SaveSettings();
        }
    }

    public bool IgnoreOutOfLeagueIncomingTrades
    {
        get => _settings.IncomingTrades.IgnoreOutOfLeague;
        set
        {
            _settings.IncomingTrades.IgnoreOutOfLeague = value;
            SaveSettings();
        }
    }

    public bool EnableSentryGeneral
    {
        get => _settings.General.EnableSentry;
        set
        {
            _settings.General.EnableSentry = value;
            SaveSettings();
        }
    }

    public bool VerifyPriceIncomingTrades
    {
        get => _settings.IncomingTrades.VerifyPrice;
        set
        {
            _settings.IncomingTrades.VerifyPrice = value;
            SaveSettings();
        }
    }
    
    public bool HighlightWithGridIncomingTrades
    {
        get => _settings.IncomingTrades.HighlightWithGrid;
        set
        {
            _settings.IncomingTrades.HighlightWithGrid = value;
            SaveSettings();
        }
    }

    public bool IgnoreSoldItemsIncomingTrades
    {
        get => _settings.IncomingTrades.IgnoreSoldItems;
        set
        {
            _settings.IncomingTrades.IgnoreSoldItems = value;
            SaveSettings();
        }
    }
    
    

    /**
     * Outgoing trades
     */
    public string ThanksWhisperOutgoingTrades
    {
        get => _settings.OutgoingTrades.ThanksWhisper;
        set
        {
            _settings.OutgoingTrades.ThanksWhisper = value;
            SaveSettings();
        }
    }

    public bool AutoThanksOutgoingTrades
    {
        get => _settings.OutgoingTrades.AutoThanks;
        set
        {
            _settings.OutgoingTrades.AutoThanks = value;
            SaveSettings();
        }
    }

    public bool AutoLeaveOutgoingTrades
    {
        get => _settings.OutgoingTrades.AutoLeave;
        set
        {
            _settings.OutgoingTrades.AutoLeave = value;
            SaveSettings();
        }
    }

    /**
     * Chaos recipe
     */
    public bool ChaosRecipeEnabled
    {
        get => _settings.ChaosRecipe.Enabled;
        set
        {
            _settings.ChaosRecipe.Enabled = value;
            SaveSettings();
        }
    }

    public int ChaosRecipeRefreshRate
    {
        get => _settings.ChaosRecipe.RefreshRate;
        set
        {
            _settings.ChaosRecipe.RefreshRate = value;
            SaveSettings();
        }
    }

    public int ChaosRecipeStashTabIndex
    {
        get => _settings.ChaosRecipe.StashTabIndex;
        set
        {
            _settings.ChaosRecipe.StashTabIndex = value;
            SaveSettings();
        }
    }

    /**
     * Chat scan
     */
    public bool ChatScanEnabled
    {
        get => _settings.ChatScan.Enabled;
        set
        {
            _settings.ChatScan.Enabled = value;
            SaveSettings();
        }
    }

    public string ChatScanWords
    {
        get => string.Join(",", _settings.ChatScan.Words);
        set
        {
            _settings.ChatScan.Words = value.Split(",")
                .Select(w => w.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();
            SaveSettings();
        }
    }

    public bool ChatScanAutoRemove
    {
        get => _settings.ChatScan.AutoRemoveMessage;
        set
        {
            _settings.ChatScan.AutoRemoveMessage = value;
            SaveSettings();
        }
    }

    public int ChatScanAutoRemoveDelay
    {
        get => _settings.ChatScan.AutoRemoveMessageDelay;
        set
        {
            _settings.ChatScan.AutoRemoveMessageDelay = value;
            SaveSettings();
        }
    }

    #endregion

    #region Constructors

    public SettingsViewModel(SettingsDto settings)
    {
        _settings = settings;
        _selectedNavigationItem = NavigationItems.First();

        _leagues
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Leagues)
            .Subscribe();

        _ = RetrieveLeagues();
    }

    #endregion

    #region Public methods

    public void Navigate(string item)
    {
        SelectedNavigationItem = item;
    }

    #endregion

    #region Private methods

    private async Task RetrieveLeagues()
    {
        var leagues = await AppService.Instance.GetLeagues();
        _leagues.AddRange(leagues);
        _loading = false;
    }

    private void SaveSettings()
    {
        if (_loading) return;
        AppService.Instance.SetSettings(_settings);
    }

    #endregion
}