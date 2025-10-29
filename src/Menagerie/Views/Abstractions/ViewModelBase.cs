using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Menagerie.ViewModels.Abstractions;

namespace Menagerie.Views.Abstractions;

public class ViewBase<T> : UserControl
    where T : ViewModelBase
{
    #region Props

    protected T? ViewModel => DataContext as T;

    #endregion

    #region Protected methods

    protected void Dispatch(Action<T?> callback)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var vm = ViewModel;
            Task.Run(() => callback.Invoke(vm));
        });
    }

    protected void InvokeUi(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    #endregion
}