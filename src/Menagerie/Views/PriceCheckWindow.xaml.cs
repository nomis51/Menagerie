using Menagerie.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Menagerie.Views {
    /// <summary>
    /// Logique d'interaction pour PriceCheck.xaml
    /// </summary>
    public partial class PriceCheckWindow : Window {
        public PriceCheckViewModel vm;

        public PriceCheckWindow() {
            InitializeComponent();

            vm = new PriceCheckViewModel();
            DataContext = vm;
        }
    }
}
