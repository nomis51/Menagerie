using System;
using Menagerie.ViewModels.Abstractions;
using Menagerie.Windows;

namespace Menagerie.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region Events

    public EventHandler<bool>? OverlayVisibilityChanged;

    #endregion

    #region Windows

    #region Incoming trades

    public IncomingTradesWindow IncomingTradesWindow { get; }

    #endregion

    #endregion

    #region Constructors

    public MainWindowViewModel(IncomingTradesWindowViewModel incomingTradesWindowViewModel)
    {
        IncomingTradesWindow = new IncomingTradesWindow
        {
            DataContext = incomingTradesWindowViewModel
        };
    }

    #endregion
}