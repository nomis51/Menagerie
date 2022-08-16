using System.Reactive.Disposables;
using Menagerie.ViewModels;
using ReactiveUI;

namespace Menagerie.Views;

public partial class ChaosRecipeContainerView
{
    #region Constructors

    public ChaosRecipeContainerView()
    {
        InitializeComponent();
        ViewModel = new ChaosRecipeContainerViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.ChaosRecipeItems,
                    x => x.ListViewChaosRecipe.ItemsSource)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion
}