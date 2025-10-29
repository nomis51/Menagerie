using Avalonia.Interactivity;
using Menagerie.ViewModels;
using Menagerie.Windows.Abstractions;

namespace Menagerie.Windows;

public partial class MainWindow : WindowBase<MainWindowViewModel>
{
    #region Constructors

    public MainWindow()
    {
        InitializeComponent();
    }

    #endregion

    #region Events

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        ViewModel!.OverlayVisibilityChanged += OnOverlayVisibilityChanged;
        ViewModel!.IncomingTradesWindow.Show(this);
    }

    private void OnOverlayVisibilityChanged(object? sender, bool isVisible)
    {
        InvokeUi(() =>
        {
            if (isVisible)
            {
                if (!ViewModel!.IncomingTradesWindow.IsVisible)
                {
                    ViewModel!.IncomingTradesWindow.Show(this);
                }
            }
            else
            {
                if (ViewModel!.IncomingTradesWindow.IsVisible)
                {
                    ViewModel!.IncomingTradesWindow.Hide();
                }
            }
        });
    }

    #endregion
}