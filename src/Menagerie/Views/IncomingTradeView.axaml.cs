using System;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.ViewModels;
using Menagerie.Views.Abstractions;

namespace Menagerie.Views;

public partial class IncomingTradeView : ViewBase<IncomingTradeViewModel>
{
    #region Constructors

    public IncomingTradeView()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    private void Border_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        Dispatch(vm =>
        {
            if (e.KeyModifiers.HasFlag(KeyModifiers.Control) && e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                vm?.AskStillInterested();
            }
            else if (e.KeyModifiers.HasFlag(KeyModifiers.Control))
            {
                vm?.SaySold();
            }
            else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
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
        Dispatch(vm => vm?.SendInvitePlayer());
    }

    private void ButtonDenyOffer_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm => vm?.SendDenyOffer());
    }

    private void OnInitialized(object? sender, EventArgs e)
    {
        Width = ViewModel!.TileSize;
        Height = ViewModel!.TileSize * 1.1;
    }

    #endregion
}