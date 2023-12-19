using Avalonia.Controls;
using Avalonia.Interactivity;
using Menagerie.ViewModels;

namespace Menagerie.Windows;

public partial class MainWindow : Window
{
    #region Members

    private IncomingOffersPanelWindow? _incomingOffersPanelWindow;

    #endregion

    #region Constructors

    public MainWindow()
    {
        InitializeComponent();

        Loaded += OnLoaded;
    }

    #endregion

    #region Private methods

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        Hide();
        _incomingOffersPanelWindow = new IncomingOffersPanelWindow
        {
            DataContext = new IncomingOffersPanelWindowViewModel()
        };
        _incomingOffersPanelWindow.Show();
    }

    #endregion
}