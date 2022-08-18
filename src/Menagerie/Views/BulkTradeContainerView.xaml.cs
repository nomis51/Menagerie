using System.Reactive.Disposables;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class BulkTradeContainerView
{
    #region Constructors

    public BulkTradeContainerView()
    {
        InitializeComponent();
        ViewModel = new BulkTradeContainerViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.BulkTradeOffers,
                    x => x.ListViewBulkTradeOffers.ItemsSource)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion

    #region Private methods
    private void ButtonTopBarClose_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        // TODO:
    }
    #endregion
}