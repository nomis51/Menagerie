using System.Drawing;
using System.Reactive.Disposables;
using System.Windows;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class SystemTrayView
{
    #region Constructors

    public SystemTrayView()
    {
        InitializeComponent();
        ViewModel = new SystemTrayViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.CurrenLeague,
                    x => x.MenuItemCurrentLeague.Header)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.AppVersion,
                    x => x.MenuItemAppVersion.Header)
                .DisposeWith(disposableRegistration);

            TaskbarIcon.Icon = new Icon("assets/menagerie-logo.ico");
        });
    }

    #endregion

    #region Private methods

    private void MenuItemQuit_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.ExitApp();
    }

    #endregion
}