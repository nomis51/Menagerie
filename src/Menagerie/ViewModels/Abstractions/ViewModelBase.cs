using System;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Menagerie.ViewModels.Abstractions;

public class ViewModelBase : ObservableObject
{
    #region Protected methods

    protected void InvokeUi(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    #endregion
}