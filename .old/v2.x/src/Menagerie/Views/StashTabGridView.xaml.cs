using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using Menagerie.ViewModels;
using ReactiveUI;
using Brushes = System.Windows.Media.Brushes;

namespace Menagerie.Views;

public partial class StashTabGridView
{
    #region Members

    private bool isFolderOffsetActive;
    private int _width;
    private int _height;
    private int _left;
    private int _top;
    private int _leftSize;
    private int _topSize;
    private string _stashTab;

    #endregion

    #region Constructors

    public StashTabGridView()
    {
        InitializeComponent();

        ViewModel = new StashTabGridViewModel();

        this.WhenActivated(disposableRegistration =>
        {
            this.OneWayBind(ViewModel,
                    x => x.IsVisible,
                    x => x.GridContent.Visibility)
                .DisposeWith(disposableRegistration);

            Margin = new Thickness(ViewModel!.StashTabGridSettings.X, ViewModel!.StashTabGridSettings.Y, 0, 0);
            Width = ViewModel!.StashTabGridSettings.Width;
            Height = ViewModel!.StashTabGridSettings.Height;
        });
    }

    #endregion

    #region Public methods

    public void Initialize(int width, int height, int left, int top, int leftSize, int topSize, string stashTab, bool hasFolderOffset)
    {
        ViewModel!.IsVisible = false;
        _stashTab = stashTab;
        _width = width;
        _height = height;
        _left = left;
        _top = top;
        _leftSize = leftSize;
        _topSize = topSize;

        GridStashGrid.Children.Clear();
        GridStashGrid.RowDefinitions.Clear();
        for (var i = 0; i < width; ++i)
        {
            GridStashGrid.RowDefinitions.Add(new RowDefinition());
        }

        GridStashGrid.ColumnDefinitions.Clear();
        for (var i = 0; i < height; ++i)
        {
            GridStashGrid.ColumnDefinitions.Add(new ColumnDefinition());
        }

        var actualTop = top - 1;
        var actualLeft = left - 1;
        var actualTopSize = actualTop + topSize - 1;
        var actualLeftSize = actualLeft + leftSize - 1;

        for (var row = 0; row < height; ++row)
        {
            for (var col = 0; col < width; ++col)
            {
                Border b = new()
                {
                    Visibility = Visibility.Hidden,
                    BorderBrush = Brushes.DarkGray,
                    BorderThickness = new Thickness(1)
                };
                Grid.SetRow(b, row);
                Grid.SetColumn(b, col);
                GridStashGrid.Children.Add(b);

                if (!((col < actualLeft || col > actualLeftSize) || (row < actualTop || row > actualTopSize))) continue;

                Rectangle rect = new()
                {
                    Visibility = ViewModel!.StashTabGridSettings.DropShadow ? Visibility.Visible : Visibility.Hidden,
                    Fill = Brushes.Black,
                    Opacity = ViewModel!.StashTabGridSettings.DropShadowOpacity,
                };
                rect.MouseDown += (sender, args) => ViewModel!.PlayClickSound();
                Grid.SetRow(rect, row);
                Grid.SetColumn(rect, col);
                GridStashGrid.Children.Add(rect);
            }
        }

        Border border = new()
        {
            Name = "BorderHighlight",
            BorderBrush = Brushes.Gold,
            BorderThickness = new Thickness(ViewModel!.StashTabGridSettings.HighlightBorderThickness)
        };
        Grid.SetRow(border, top - 1);
        Grid.SetRowSpan(border, topSize);
        Grid.SetColumn(border, left - 1);
        Grid.SetColumnSpan(border, leftSize);
        GridStashGrid.Children.Add(border);

        ButtonStashTab.Content = stashTab;

        isFolderOffsetActive = hasFolderOffset;
        switch (!isFolderOffsetActive)
        {
            case true when !hasFolderOffset:
            case false when hasFolderOffset:
                ApplyFolderOffset(false);
                break;
        }

        ViewModel!.IsVisible = true;
    }

    #endregion

    #region Private methods

    private void ButtonClose_Click(object sender, RoutedEventArgs e)
    {
        ViewModel!.PlayClickSound();
        ViewModel!.IsVisible = false;
    }

    private void ButtonIncreaseWidth_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridSize(1, 0);
    }

    private void ButtonDecreaseWidth_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridSize(-1, 0);
    }

    private void ButtonIncreaseHeight_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridSize(0, 1);
    }

    private void ButtonDecreaseHeight_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridSize(0, -1);
    }

    private void ButtonIncreaseX_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridPosition(1, 0);
    }

    private void ButtonDecreaseX_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridPosition(-1, 0);
    }

    private void ButtonDecreaseY_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridPosition(0, -1);
    }

    private void ButtonIncreaseY_OnClick(object sender, RoutedEventArgs e)
    {
        UpdateGridPosition(0, 1);
    }

    private void ButtonResize_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleGrid();
    }

    private void ButtonToggleDropShadow_Click(object sender, RoutedEventArgs e)
    {
        ViewModel!.PlayClickSound();
        foreach (var element in GridStashGrid.Children)
        {
            if (element is not Rectangle rect) continue;
            rect.Visibility = rect.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        }
    }

    private void UpdateGridPosition(double x, double y)
    {
        ViewModel!.PlayClickSound();
        Margin = new Thickness(Margin.Left + (x * ViewModel!.StashTabGridSettings.IncrementValue), Margin.Top + (y * ViewModel!.StashTabGridSettings.IncrementValue),
            Margin.Right, Margin.Bottom);
        ViewModel?.UpdatePosition((int)Margin.Left, (int)Margin.Top);
    }

    private void UpdateGridSize(int width, int height)
    {
        ViewModel!.PlayClickSound();
        Width += (width * ViewModel!.StashTabGridSettings.IncrementValue);
        Height += (height * ViewModel!.StashTabGridSettings.IncrementValue);
        ViewModel?.UpdateSize((int)Width, (int)Height);
        UpdateGridPosition(0, height);
    }

    private void ToggleGrid()
    {
        ViewModel!.PlayClickSound();
        StackPanelAdjustWidth.Visibility = StackPanelAdjustWidth.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        StackPanelAdjustX.Visibility = StackPanelAdjustX.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        DockPanelAdjustHeight.Visibility = DockPanelAdjustHeight.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
        DockPanelAdjustY.Visibility = DockPanelAdjustY.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;

        foreach (UIElement child in GridStashGrid.Children)
        {
            if (child is Border border)
            {
                if (border.Name == "BorderHighlight") continue;
                border.Visibility = StackPanelAdjustWidth.Visibility;
            }
        }
    }

    private void ButtonToggleFolder_OnClick(object sender, RoutedEventArgs e)
    {
        isFolderOffsetActive = !isFolderOffsetActive;
        ApplyFolderOffset();
    }

    private void ApplyFolderOffset(bool save = true)
    {
        Margin = new Thickness(
            Margin.Left,
            isFolderOffsetActive ? ViewModel!.StashTabGridSettings.FolderOffset + Margin.Top : ViewModel!.StashTabGridSettings.Y,
            Margin.Right,
            Margin.Bottom
        );

        if (!save) return;
        ViewModel?.SaveStashTabGridType(_stashTab, _width, _height, isFolderOffsetActive);
    }

    private void ButtonToggleGridType_OnClick(object sender, RoutedEventArgs e)
    {
        ToggleTabGridType();
    }

    private void ToggleTabGridType()
    {
        Initialize(_width == 12 ? 48 : 12, _height == 12 ? 48 : 12, _left, _top, _leftSize, _topSize, _stashTab, isFolderOffsetActive);
        ViewModel?.SaveStashTabGridType(_stashTab, _width, _height, isFolderOffsetActive);
    }

    #endregion
}