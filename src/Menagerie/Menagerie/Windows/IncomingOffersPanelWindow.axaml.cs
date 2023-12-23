using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.Core;
using Menagerie.ViewModels;

namespace Menagerie.Windows;

public partial class IncomingOffersPanelWindow : WindowBase<IncomingOffersPanelWindowViewModel>
{
    #region Constructors

    public IncomingOffersPanelWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Events.HideOverlay += OnHideOverlay;
        Events.ShowOverlay += OnShowOverlay;
    }

    #endregion

    #region Private methods

    private void OnShowOverlay()
    {
        InvokeUi(Show);
    }

    private void OnHideOverlay()
    {
        InvokeUi(Hide);
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        AdjustPosition();
    }

    private void AdjustPosition()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Left hud and right hud are using about 29% of the screen width
            // which gives the exp bar portion about 41% of the screen width
            // the exp bar is using about 2.5% of the screen height
            // the space above the exp bar is about 11% of the screen height
            // which gives about 7.5% of the screen height space for the panel

            var size = desktop.MainWindow!.Screens.Primary!.Bounds.Size;
            Position = new PixelPoint((int)(size.Width * .278), (int)(size.Height * .962 - Height));
            Height = (int)(size.Height * .092);
            Width = 25; //(int)(size.Width * .444);
            MaxWidth = (int)(size.Width *.444);
            ViewModel?.SetOfferSize((int)(size.Height * .092 - ButtonRemoveAllOffers.Height));
            Panel.Width = 0;
        }
    }

    private void ScrollViewer_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        switch (e.Delta.Y)
        {
            case > 0:
                ScrollViewer.PageLeft();
                break;

            case < 0:
                ScrollViewer.PageRight();
                break;
        }
    }

    private void ButtonRemoveAllOffers_OnClick(object? sender, RoutedEventArgs e)
    {
        Dispatch(vm=>vm?.RemoveAllOffers());
    }
    #endregion
}