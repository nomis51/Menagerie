using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.ViewModels;

namespace Menagerie.Views;

public partial class IncomingOffer : ViewBase<IncomingOfferViewModel>
{
    #region Constructors

    public IncomingOffer()
    {
        InitializeComponent();

        Initialized += OnInitialized;
    }

    #endregion

    #region Private methods

    private void OnInitialized(object? sender, EventArgs e)
    {
        Width = ViewModel!.Width;
    }

    private void Border_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Dispatch(vm => vm?.DoNextAction());
    }

    private void ButtonBusy_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.SayBusy());
    }

    private void ButtonReInvitePlayer_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.InvitePlayer());
    }

    private void ButtonDenyOffer_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.DenyOffer());
    }

    #endregion
}