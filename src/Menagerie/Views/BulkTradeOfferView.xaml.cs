using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Menagerie.Views;

public partial class BulkTradeOfferView
{
    #region Members
    private readonly SolidColorBrush _green;
    private readonly SolidColorBrush _yellow;
    #endregion

    #region Constructors

    public BulkTradeOfferView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.PayCurrency,
                    x => x.TextBlockPayCurrency.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.PayAmount,
                    x => x.TextBlockPayAmount.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.PayCurrentImage,
                    x => x.ImagePayCurrency.Source,
                    x => new BitmapImage(x))
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.GetCurrency,
                    x => x.TextBlockGetCurrency.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.GetAmount,
                    x => x.TextBlockGetAmount.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.GetCurrentImage,
                    x => x.ImageGetCurrency.Source,
                    x => new BitmapImage(x))
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Player,
                    x => x.TextBlockPlayer.Text)
                .DisposeWith(disposableRegistration);
        });

        _green = (SolidColorBrush)FindResource("Green");
        _yellow = (SolidColorBrush)FindResource("Yellow");
    }

    #endregion

    #region Private methods

    private void ButtonSendWhisper_OnClick(object sender, RoutedEventArgs e)
    {
        BorderContent.BorderBrush = _green;
        ViewModel?.SendWhisper();
    }

    private void ButtonWhisperPlayer_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.WhisperPlayer();
    }

    private void GridContent_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!ViewModel!.Whispered) return;
        BorderContent.BorderBrush = _yellow;
    }
    #endregion

}