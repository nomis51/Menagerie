using System.Reactive.Disposables;
using System.Windows;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class IncomingOffersContainerView
{
    #region Constructors

    public IncomingOffersContainerView()

    {
        InitializeComponent();

        ViewModel = new IncomingOffersContainerViewModel();
        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.IncomingOffers,
                    view => view.ListViewIncomingOffers.ItemsSource)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion

    #region Private methods

    private void ButtonRemoveAllIncomingOffers_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.RemoveAllIncomingOffers();
    }

    #endregion
}