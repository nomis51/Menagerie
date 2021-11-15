using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Menagerie.ViewModels;

namespace Menagerie.Views
{
    public partial class ChaosRecipeView : UserControl
    {
        #region Members

        private ChaosRecipeViewModel Vm => (ChaosRecipeViewModel)DataContext;

        #endregion

        #region Constructors

        public ChaosRecipeView()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        #endregion

        #region Handlers

        private void BtnChangeDockChaosRecipeOrientation_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void BtnChangeStackChaosRecipeOrientation_OnClick(object sender, RoutedEventArgs e)
        {
        }

        private void GrdChaosRecipe_MouseEnter(object sender, MouseEventArgs e)
        {
            SetPanelOpacity(0.1d);
        }

        private void GrdChaosRecipe_MouseLeave(object sender, MouseEventArgs e)
        {
            SetPanelOpacity(0.7d);
        }

        #endregion

        #region Private methods

        private void SetPanelOpacity(double value)
        {
            GrdChaosRecipe.Opacity = value;
        }
        #endregion
    }
}