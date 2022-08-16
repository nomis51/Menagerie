using System.Reactive.Disposables;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class ChatScanContainerView
{
    #region Constructors

    public ChatScanContainerView()
    {
        InitializeComponent();
        ViewModel = new ChatScanContainerViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.ChatScanMessages,
                    x => x.ListViewChatMessages.ItemsSource)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion
}