using System.Reactive.Disposables;
using System.Windows;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class NavigationView
{
    #region Constructors

    public NavigationView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.NavigationItems,
                    x => x.ListViewNavigationItems.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.AreNavigationItemsVisible,
                    x => x.ListViewNavigationItems.Visibility)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion

    #region Private methods

    private void ButtonTools_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.ToggleToolsButtons();
    }

    #endregion
}