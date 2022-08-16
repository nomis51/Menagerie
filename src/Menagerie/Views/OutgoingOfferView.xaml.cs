using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Menagerie.Shared.Constants;
using ReactiveUI;

namespace Menagerie.Views;

public partial class OutgoingOfferView
{
    private readonly SolidColorBrush _green = (SolidColorBrush)new BrushConverter().ConvertFrom(AppTheme.Green)!;

    private readonly SolidColorBrush _darkBackground =
        (SolidColorBrush)new BrushConverter().ConvertFrom(AppTheme.DarkBackground)!;

    public OutgoingOfferView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.ItemNameTrimmed,
                    x => x.LabelItemName.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Player,
                    x => x.LabelPlayerName.Content)
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
                x => x.HasJoinedHideout,
                x => x.BorderContent.BorderBrush,
                x => x ? _green : _darkBackground)
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
                x => x.ItemName,
                x => x.ItemImage.ToolTip)
            .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
               x => x.ItemImageUri,
               x => x.ItemImage.Source)
           .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                x => x.HasImage,
                x => x.ItemImage.Visibility)
            .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
               x => x.HasImage,
               x => x.LabelItemName.Visibility,
               v=>v ? Visibility.Hidden : Visibility.Visible)
           .DisposeWith(disposableRegistration);

            // TODO: when outgoing is able to say if it's bulk trade, then uncomment this
            // this.OneWayBind(ViewModel,
            //        x => x.Price,
            //        x => x.LabelTargetPrice.Content)
            //    .DisposeWith(disposableRegistration);
            //
            // this.OneWayBind(ViewModel,
            //         x => x.CurrencyImageUri,
            //         x => x.ImageTargetCurrency.Source)
            //     .DisposeWith(disposableRegistration);
        });
    }

    private void BtnJoinHideout_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.SendHideoutCommand();
    }

    private void BtnWhisper_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.PrepareToSendWhisper();
    }

    private void BtnTrade_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.SendTradeRequest();
    }

    private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.RemoveOffer();
    }

    private void GridContent_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        ViewModel?.SetNextOfferState();
    }
}