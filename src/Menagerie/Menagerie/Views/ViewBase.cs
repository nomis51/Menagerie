using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Menagerie.ViewModels;

namespace Menagerie.Views;

public class ViewBase<T> : UserControl
    where T : ViewModelBase
{
    #region Props

    public T? ViewModel => DataContext as T;

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