using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Menagerie.ViewModels;
using Menagerie.Windows.Abstractions;

namespace Menagerie.Windows;

public partial class IncomingTradesWindow : WindowBase<IncomingTradesWindowViewModel>
{
    #region Constructors

    public IncomingTradesWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        // AppService.Instance.AddOverlayWindowHandle(GetTopLevel(this)!.TryGetPlatformHandle()!.Handle);
        AdjustPosition();
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

    private void ButtonRemoveAllTrades_OnClick(object? sender, RoutedEventArgs e)
    {
        ViewModel?.RemoveAllTrades();
    }

    #endregion

    #region Private methods

    private void AdjustPosition()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Left hud and right hud are using about 29% of the screen width
            // which gives the exp bar portion about 41% of the screen width
            // the exp bar is using about 2.5% of the screen height
            // the space above the exp bar is about 11% of the screen height
            // which gives about 7.5% of the screen height space for the panel

            var size = desktop.MainWindow!.Screens.Primary is null
                ? desktop.MainWindow!.Screens.ScreenFromWindow(this)!.Bounds.Size
                : desktop.MainWindow!.Screens.Primary.Bounds.Size;
            Position = new PixelPoint((int)(size.Width * .278), (int)(size.Height * .962 - Height));
            Height = (int)(size.Height * .092);
            Width = (int)(size.Width * .444);
            MaxWidth = (int)(size.Width * .88);
            ViewModel!.TradeTileSize = ((int)(size.Height * .092 - ButtonRemoveAllTrades.Height));
        }
    }

    #endregion
}