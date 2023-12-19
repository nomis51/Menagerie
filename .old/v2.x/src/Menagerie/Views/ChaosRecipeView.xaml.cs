using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ReactiveUI;

namespace Menagerie.Views;

public partial class ChaosRecipeView
{
    public ChaosRecipeView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.IconLinkUri,
                    x => x.ImageCategory.Source,
                    x => new BitmapImage(x))
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Count,
                    x => x.LabelCount.Content)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                x => x.HasIconLink,
                x => x.ImageCategory.Visibility)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                x => x.HasIconLink,
                x => x.BorderCount.VerticalAlignment,
                x => x ? VerticalAlignment.Bottom : VerticalAlignment.Center)
                .DisposeWith(disposableRegistration);
            
            this.OneWayBind(ViewModel,
                x => x.HasIconLink,
                x => x.BorderCount.HorizontalAlignment,
                x => x ? HorizontalAlignment.Right : HorizontalAlignment.Center)
                .DisposeWith(disposableRegistration);
        });
    }
}