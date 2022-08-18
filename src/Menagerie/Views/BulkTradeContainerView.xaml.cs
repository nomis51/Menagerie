using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Forms;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class BulkTradeContainerView
{
    #region Events

    public delegate void CloseEvent();

    public event CloseEvent OnClose;

    #endregion

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

            this.OneWayBind(ViewModel,
                    x => x.Currencies,
                    x => x.ComboBoxGetCurrency.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Currencies,
                    x => x.ComboBoxPayCurrency.ItemsSource)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.Have,
                    x => x.ComboBoxPayCurrency.SelectedValue)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.Want,
                    x => x.ComboBoxGetCurrency.SelectedValue)
                .DisposeWith(disposableRegistration);

            this.Bind(ViewModel,
                    x => x.MinGet,
                    x => x.TextBoxMinGet.Text)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsLoading,
                    x => x.ButtonSearch.IsEnabled,
                    (bool x) => !x)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.IsLoading,
                    x => x.ProgressBarLoading.Visibility)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion

    #region Private methods

    private void ButtonTopBarClose_Click(object sender, RoutedEventArgs e)
    {
        OnClose?.Invoke();
    }

    private void ButtonSearch_OnClick(object sender, RoutedEventArgs e)
    {
        _ = ViewModel?.Search();
    }

    #endregion
}