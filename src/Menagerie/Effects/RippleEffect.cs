using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Menagerie.Effects;

public class RippleEffect : ContentControl
{
    static RippleEffect()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RippleEffect),
            new FrameworkPropertyMetadata(typeof(RippleEffect)));
    }

    public Brush HighlightBackground
    {
        get => (Brush)GetValue(HighlightBackgroundProperty);
        set => SetValue(HighlightBackgroundProperty, value);
    }

    public static readonly DependencyProperty HighlightBackgroundProperty =
        DependencyProperty.Register(nameof(HighlightBackground), typeof(Brush), typeof(RippleEffect),
            new PropertyMetadata(Brushes.White));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        var ellipse = GetTemplateChild("PartEllipse") as Ellipse;
        var grid = GetTemplateChild("PartGrid") as Grid;
        var animation = grid?.FindResource("PartAnimation") as Storyboard;

        AddHandler(MouseDownEvent, new RoutedEventHandler((_, e) =>
        {
            var targetWidth = Math.Max(ActualWidth, ActualHeight) * 2;
            var mousePosition = (e as MouseButtonEventArgs)!.GetPosition(this);
            var startMargin = new Thickness(mousePosition.X, mousePosition.Y, 0, 0);
            if (ellipse == null) return;
            ellipse.Margin = startMargin;
            (animation?.Children[0] as DoubleAnimation)!.To = targetWidth;
            (animation.Children[1] as ThicknessAnimation)!.From = startMargin;
            (animation.Children[1] as ThicknessAnimation)!.To = new Thickness(mousePosition.X - targetWidth / 2,
                mousePosition.Y - targetWidth / 2, 0, 0);
            ellipse.BeginStoryboard(animation);
        }), true);
    }
}