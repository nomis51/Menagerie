using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Menagerie.Shared.Constants;
using ReactiveUI;

namespace Menagerie.Views;

public partial class IncomingOfferView
{
    #region Members

    private readonly SolidColorBrush _darkBackground;
    private readonly SolidColorBrush _green;
    private readonly SolidColorBrush _blue;
    private readonly SolidColorBrush _orange;
    private readonly SolidColorBrush _yellow;

    #endregion

    public IncomingOfferView()
    {
        InitializeComponent();

        _green = (SolidColorBrush)FindResource("Green");
        _blue = (SolidColorBrush)FindResource("Blue");
        _orange = (SolidColorBrush)FindResource("Orange");
        _yellow = (SolidColorBrush)FindResource("Yellow");
        _darkBackground = (SolidColorBrush)FindResource("DarkBackground");


        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.ItemNameTrimmed,
                    x => x.LblItemName.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Price,
                    x => x.LabelPrice.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.CurrencyImageUri,
                    x => x.ImageCurrency.Source)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsPlayerInvited,
                    x => x.BtnBusy.Visibility,
                    x => x ? Visibility.Hidden : Visibility.Visible)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsPlayerInvited,
                    x => x.BtnReInvite.Visibility)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.HasSaidBusy,
                    x => x.BorderContent.BorderBrush,
                    x => StateToBrush())
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsPlayerInvited,
                    x => x.BorderContent.BorderBrush,
                    x => StateToBrush())
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.HasSentTradeRequest,
                    x => x.BorderContent.BorderBrush,
                    x => StateToBrush())
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.HasPlayerJoined,
                    x => x.IconPlayerJoined.Visibility,
                    x => x ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.HasPlayerJoined,
                    x => x.BorderContent.BorderBrush,
                    x => StateToBrush())
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsScam,
                    x => x.IconScam.Visibility,
                    x => x ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Player,
                    x => x.LabelTooltipPlayer.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.League,
                    x => x.LabelTooltipLeague.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.PriceStr,
                    x => x.LabelTooltipPrice.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Time,
                    x => x.LabelTooltipTime.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ItemName,
                    x => x.LabelTooltipItemName.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsScam,
                    x => x.DockTooltipScam.Visibility)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.ScamPrice,
                    x => x.RunTooltipScamTradePrice.Text)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.PriceStr,
                    x => x.RunTooltipScamPlayerPrice.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.PriceConversions,
                    x => x.LabelTooltipPriceConversions.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.HasPriceConversions,
                    x => x.DockTooltipPriceConversions.Visibility)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.ElapsedTime,
                    x => x.LabelTooltipElapsedTime.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.PriceFontSize,
                    x => x.LabelPrice.FontSize)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.StashTab,
                    x => x.LabelTooltipStashTab.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Location,
                    x => x.LabelTooltipLocation.Content)
                .DisposeWith(disposableRegistration);
        });
    }

    private SolidColorBrush StateToBrush()
    {
        if (ViewModel is null) return _darkBackground;
        if (ViewModel.HasSentTradeRequest) return _orange;
        if (ViewModel.HasPlayerJoined) return _yellow;
        if (ViewModel.IsPlayerInvited) return _green;
        return ViewModel.HasSaidBusy ? _blue : _darkBackground;
    }

    private void GridContent_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;

        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                ViewModel?.AskStillInterested();
                return;
            }

            ViewModel?.SaySold();
            return;
        }

        if (Keyboard.IsKeyDown(Key.LeftAlt))
        {
            ViewModel?.HighlightItem();
            return;
        }

        if (Keyboard.IsKeyDown(Key.LeftShift))
        {
            ViewModel?.PrepareToSendWhisper();
            return;
        }

        ViewModel?.SetNextOfferState();
    }

    private void BtnBusy_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.SayBusy();
    }

    private void BtnReInvite_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.ReInvite();
    }

    private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.Kick();
    }
}