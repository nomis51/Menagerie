using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.ViewModels;

namespace Menagerie.Views;

public partial class IncomingOffer : ViewBase<IncomingOfferViewModel>
{
    #region Members

    private bool _isControlDown;
    private bool _isShiftDown;

    #endregion

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
        Dispatch(vm =>
        {
            if (_isControlDown && _isShiftDown)
            {
                vm?.AskStillInterested();
            }
            else if (_isControlDown)
            {
                vm?.SaySold();
            }
            else if (_isShiftDown)
            {
                vm?.Whisper();
            }
            else
            {
                vm?.DoNextAction();
            }
        });
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

    private void Border_OnKeyDown(object? sender, KeyEventArgs e)
    {
        _isControlDown = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        _isShiftDown = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
    }

    private void Border_OnKeyUp(object? sender, KeyEventArgs e)
    {
        _isControlDown = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        _isShiftDown = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
    }

    #endregion
}