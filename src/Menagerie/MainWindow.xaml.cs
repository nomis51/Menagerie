using System;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Menagerie.ViewModels;
using ReactiveUI;
using Serilog;

namespace Menagerie;

public partial class MainWindow
{

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    private const int GWL_EX_STYLE = -20;
    private const int WS_EX_APPWINDOW = 0x00040000, WS_EX_TOOLWINDOW = 0x00000080;
    
    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new AppViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.NavigationViewModel,
                    x => x.NavigationView.ViewModel)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsChaosRecipeVisible,
                    x => x.ChaosRecipeContainerView.Visibility,
                    x => x ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    viewModel => viewModel.IsStatsVisible,
                    x => x.TradesStatisticsView.Visibility,
                    x => x ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.IsTranslationToolVisible,
                    x => x.TranslationTool.Visibility,
                    x => x ? Visibility.Visible : Visibility.Hidden)
                .DisposeWith(disposableRegistration);


            ViewModel.OnOverlayVisibilityChange += ViewModel_OnOverlayVisibilityChange;
            ViewModel.OnStashGridVisibilityChange += ViewModel_OnStashGridVisibilityChange;
            ViewModel.OnTranslatorVisible += ViewModel_OnTranslatorVisible;

            TradesStatisticsView.OnClose += TradesStatisticsView_OnClose;
            TranslationTool.OnClose += TranslationTool_OnClose;
        });

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        var handle =  new WindowInteropHelper(this).Handle;
        ViewModel?.SetOverlayHandle(handle);
    }


    private void ViewModel_OnTranslatorVisible()
    {
        TranslationTool.FocusTextBox();
    }

    private void ViewModel_OnStashGridVisibilityChange(bool isVisible, int width, int height, int left, int top, int leftSize, int topSize, string stashTab, bool hasFolderOffset = false)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            StashTabGridView.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            if (!isVisible) return;

            StashTabGridView.Initialize(width, height, left, top, leftSize, topSize, stashTab, hasFolderOffset);
        });
    }

    private void TranslationTool_OnClose()
    {
        ViewModel?.ToggleTranslator();
    }

    private void TradesStatisticsView_OnClose()
    {
        ViewModel?.ToggleStatistics();
    }

    private void ViewModel_OnOverlayVisibilityChange(bool isVisible)
    {
        try
        {
            switch (isVisible)
            {
                case true when Visibility != Visibility.Visible:
                    System.Windows.Application.Current.Dispatcher.Invoke(Show);
                    break;
                case false when Visibility == Visibility.Visible:
                    System.Windows.Application.Current.Dispatcher.Invoke(Hide);
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Warning("Unable to set overlay visibility: {message}. {stackTrace}", e.Message, e.StackTrace);
        }
    }
}