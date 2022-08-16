using System;
using ReactiveUI;

namespace Menagerie.Extensions;

public static class ViewModelExtensions
{
    public static TView GetWindowView<TViewModel, TView>(this TViewModel viewModel)
        where TViewModel : ReactiveObject where TView : ReactiveWindow<TViewModel>
    {
        var view = Activator.CreateInstance<TView>();
        view.ViewModel = viewModel;
        return view;
    }
}