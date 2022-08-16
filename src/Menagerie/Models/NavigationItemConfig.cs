using System;
using System.Windows;
using System.Windows.Media;
using FontAwesome5;

namespace Menagerie.Models;

public class NavigationItemConfig
{
    public Brush Background { get; set; }
    public Brush BorderBrush { get; set; }
    public Thickness Margin { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Style Style { get; set; }
    public NavigationItemIconConfig IconConfig { get; set; }
    public Action<object, RoutedEventArgs> OnClickFn { get; set; }
}

public class NavigationItemIconConfig
{
    public EFontAwesomeIcon Icon { get; set; }
    public Thickness Margin { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Brush Foreground { get; set; }
}