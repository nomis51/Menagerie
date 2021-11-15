using System.Collections.Generic;
using System.Windows;
using Caliburn.Micro;
using Menagerie.Models;
using Serilog;

namespace Menagerie.ViewModels
{
    public class ChaosRecipeViewModel : Screen
    {
        #region Members

        #endregion

        #region Props

        public ReactiveProperty<bool> IsDockLayout;
        public ReactiveProperty<BindableCollection<ChaosRecipeItem>> ChaosRecipeItems;
        public Visibility DockLayoutVisibility => IsDockLayout.Value ? Visibility.Visible : Visibility.Hidden;
        public Visibility StackLayoutVisibility => IsDockLayout.Value ? Visibility.Visible : Visibility.Hidden;
        public int LayoutWidth => IsDockLayout.Value ? 530 : 60;
        public int LayoutHeight => IsDockLayout.Value ? 40 : 380;

        #endregion

        #region Constructors

        public ChaosRecipeViewModel()
        {
            Log.Information("Initializing OverlayViewModel");

            ChaosRecipeItems =
                new ReactiveProperty<BindableCollection<ChaosRecipeItem>>(nameof(ChaosRecipeItems), this, new BindableCollection<ChaosRecipeItem>());
            IsDockLayout = new ReactiveProperty<bool>(nameof(IsDockLayout), this, true)
            {
                AdditionalPropertiesToNotify = new List<string>
                {
                    nameof(DockLayoutVisibility),
                    nameof(StackLayoutVisibility),
                    nameof(LayoutWidth),
                    nameof(LayoutHeight)
                }
            };
        }

        #endregion
    }
}