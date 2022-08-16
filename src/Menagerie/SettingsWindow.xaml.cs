using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ReactiveUI;
using ListBox = System.Windows.Controls.ListBox;

namespace Menagerie;

public partial class SettingsWindow
{
    public SettingsWindow()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.AppVersion,
                    x => x.LabelAppVersion.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.NavigationItems,
                    x => x.ListBoxNavigation.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.SelectedNavigationItem,
                    x => x.StackPanelGeneral.Visibility,
                    x => x == "General" ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.SelectedNavigationItem,
                    x => x.StackPanelIncomingTrades.Visibility,
                    x => x == "Incoming trades" ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.SelectedNavigationItem,
                    x => x.StackPanelOutgoingTrades.Visibility,
                    x => x == "Outgoing trades" ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.SelectedNavigationItem,
                    x => x.StackPanelChaosRecipe.Visibility,
                    x => x == "Chaos recipe" ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.SelectedNavigationItem,
                    x => x.StackPanelChatScan.Visibility,
                    x => x == "Chat scan" ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            // General
            this.Bind(ViewModel,
                    x => x.AccountName,
                    x => x.TextBoxAccountName.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Poesessid,
                    x => x.TextBoxPoesessid.Password)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.League,
                    x => x.ComboBoxLeague.SelectedValue)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Leagues,
                    x => x.ComboBoxLeague.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.EnableRecording,
                    x => x.ButtonEnableRecording.IsChecked)
                .DisposeWith(disposableRegistration);

            // Incoming trades
            this.Bind(ViewModel,
                    x => x.BusyWhisperIncomingTrades,
                    x => x.TextBoxBusyWhisperIncomingTrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.SoldWhisperIncomingTrades,
                    x => x.TextBoxSoldWhisperIncomingTrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.StillInterestedWhisperIncomingTrades,
                    x => x.TextBoxStillInterestedWhisperIncomingTrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ThanksWhisperIncomingTrades,
                    x => x.TextBoxThanksWhisperIncomingTrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.InviteWhisperIncomingTrades,
                    x => x.TextBoxInviteWhisperIncomingtrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.AutoThanksIncomingTrades,
                    x => x.ButtonAutoThanksIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.AutoKickIncomingTrades,
                    x => x.ButtonAutoKickIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.IgnoreOutOfLeagueIncomingTrades,
                    x => x.ButtonIgnoreOutOfLeagueIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.VerifyPriceIncomingTrades,
                    x => x.ButtonVerifyPriceIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.IgnoreSoldItemsIncomingTrades,
                    x => x.ButtonIgnoreSoldIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.HighlightWithGridIncomingTrades,
                    x => x.ButtonHighlightWithGridIncomingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            // Outgoing trades
            this.Bind(ViewModel,
                    x => x.ThanksWhisperOutgoingTrades,
                    x => x.TextBoxThanksWhisperOutgoingTrades.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.AutoThanksOutgoingTrades,
                    x => x.ButtonAutoThanksOutgoingTrades.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                x => x.AutoLeaveOutgoingTrades,
                x => x.ButtonAutoLeaveOutgoingTrades.IsChecked);

            // Chaos recipe
            this.Bind(ViewModel,
                    x => x.ChaosRecipeEnabled,
                    x => x.ButtonEnableChaosRecipe.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ChaosRecipeStashTabIndex,
                    x => x.TextBoxStashTabIndexChaosRecipe.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ChaosRecipeRefreshRate,
                    x => x.TextBoxRefreshRateChaosRecipe.Text)
                .DisposeWith(disposableRegistration);

            // Chat scan
            this.Bind(ViewModel,
                    x => x.ChatScanEnabled,
                    x => x.ButtonEnableChatScan.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ChatScanWords,
                    x => x.TextBoxWordsChatScan.Text)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ChatScanAutoRemove,
                    x => x.ButtonEnableAutoRemoveMessageChatScan.IsChecked)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.ChatScanAutoRemoveDelay,
                    x => x.TextBoxAutoRemoveDelayChatScan.Text)
                .DisposeWith(disposableRegistration);
        });
    }

    private void ButtonTopBarClose_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    // private void ButtonTopBarMaximize_OnClick(object sender, RoutedEventArgs e)
    // {
    //     WindowState = WindowState == WindowState.Maximized
    //         ? WindowState.Normal
    //         : WindowState.Maximized;
    // }
    //
    // private void ButtonTopBarMinimize_OnClick(object sender, RoutedEventArgs e)
    // {
    //     WindowState = WindowState.Minimized;
    // }

    private void ListBoxNavigation_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender;
        ViewModel?.Navigate((string)listBox.SelectedValue);
    }

    // 2way binding doesn't work well with PasswordBox
    private void TextBoxPoesessid_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (ViewModel != null) ViewModel.Poesessid = ((PasswordBox)sender).Password;
    }
}