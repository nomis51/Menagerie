using Menagerie.ViewModels;
using Menagerie.Windows.Abstractions;

namespace Menagerie.Windows;

public partial class MainWindow : WindowBase<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
    }
}