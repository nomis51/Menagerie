using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;

namespace Menagerie.Views;

public partial class ChatScanMessageView
{
    public ChatScanMessageView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.PlayerName,
                    x => x.LabelPlayerName.Content)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.Time,
                    x => x.LabelTime.Content)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.Message,
                    x => x.TextBlockMessage.Text)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.Type,
                    x => x.TextBlockMessageType.Text)
                .DisposeWith(disposableRegistration);
        });
    }

    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
        ((Button)sender).IsEnabled = false;
        ViewModel?.Remove();
    }
}