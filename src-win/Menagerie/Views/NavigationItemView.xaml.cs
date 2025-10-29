using System.Reactive.Disposables;
using System.Windows;
using ReactiveUI;

namespace Menagerie.Views;

public partial class NavigationItemView
{
    #region Constructors

    public NavigationItemView()
    {
        InitializeComponent();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.Config.Background,
                    x => x.Button.Background)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.Height,
                    x => x.Button.Height)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.Width,
                    x => x.Button.Width)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.Margin,
                    x => x.Button.Margin)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.Style,
                    x => x.Button.Style)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.BorderBrush,
                    x => x.Button.BorderBrush)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.IconConfig.Foreground,
                    x => x.ImageAwesome.Foreground)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.IconConfig.Height,
                    x => x.ImageAwesome.Height)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.IconConfig.Width,
                    x => x.ImageAwesome.Width)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.IconConfig.Icon,
                    x => x.ImageAwesome.Icon)
                .DisposeWith(disposableRegistration);

            this.OneWayBind(ViewModel,
                    x => x.Config.IconConfig.Margin,
                    x => x.ImageAwesome.Margin)
                .DisposeWith(disposableRegistration);
        });
    }

    #endregion

    #region Private methods

    private void Button_OnClick(object sender, RoutedEventArgs e)
    {
        ViewModel?.Config.OnClickFn(sender, e);
    }

    #endregion
}