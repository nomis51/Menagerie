using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Menagerie.Core.WinApi;
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
        AdjustWindow();

        _incomingOffersPanelWindow = new IncomingOffersPanelWindow
        {
            DataContext = new IncomingOffersPanelWindowViewModel()
        };
        _incomingOffersPanelWindow.Show(this);
    }

    #endregion

    #region Private methods

    private void AdjustWindow()
    {
        Position = new PixelPoint(-100, -100);

        // TODO: implement exstyle for Linux
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var hwnd = GetTopLevel(this)!.TryGetPlatformHandle()!.Handle;
        var windowStyle = User32.GetWindowLong(hwnd, User32.GWL_EX_STYLE);
        windowStyle |= User32.WS_EX_TOOLWINDOW;
        _ = User32.SetWindowLong(hwnd, User32.GWL_EX_STYLE, windowStyle);
    }

    #endregion
}