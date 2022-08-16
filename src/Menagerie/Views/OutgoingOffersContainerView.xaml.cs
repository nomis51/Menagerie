using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Menagerie.Helpers;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class OutgoingOffersContainerView
{
    #region Members

    private readonly TextBoxDebouncer _searchOutgoingOfferTypingAssitant;

    #endregion

    #region Constructors

    public OutgoingOffersContainerView()
    {
        InitializeComponent();
        ViewModel = new OutgoingOffersContainerViewModel();

        _searchOutgoingOfferTypingAssitant = new TextBoxDebouncer();
        _searchOutgoingOfferTypingAssitant.OnIdled += SearchOutgoingOfferTypingAssitant_OnIdled;

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    viewModel => viewModel.OutgoingOffers,
                    view => view.ListViewOutgoingOffers.ItemsSource)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                    x => x.IsSearchOutgoingOfferVisible,
                    x => x.Visibility)
                .DisposeWith(disposableRegistration);

            ViewModel.OnFocusSearchOutgoingOffer += ViewModelOnOnFocusSearchOutgoingOffer;
        });
    }

    #endregion

    #region Private methods

    private void ViewModelOnOnFocusSearchOutgoingOffer()
    {
        Keyboard.Focus(TextBoxSearchOutgoingOffer);
    }

    private void SearchOutgoingOfferTypingAssitant_OnIdled(object? sender, EventArgs e)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate { ViewModel?.ShowOutgoingOffers(TextBoxSearchOutgoingOffer.Text); });
    }

    private void ButtonCloseSearchOutgoingOffer_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel!.ToggleSearchOutgoingOffer();
    }

    #endregion

    private void TextBoxSearchOutgoingOffer_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchOutgoingOfferTypingAssitant.TextChanged();
    }
}