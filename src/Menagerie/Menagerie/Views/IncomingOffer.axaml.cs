using System;
using Menagerie.ViewModels;

namespace Menagerie.Views;

public partial class IncomingOffer : ViewBase<IncomingOfferViewModel>
{
    #region Constructors

    public IncomingOffer()
    {
        InitializeComponent();

        Initialized += OnInitialized;
    }

    #endregion

    #region Private methods

    private void OnInitialized(object? sender, EventArgs e)
    {
        Width = ViewModel!.Width;
    }

    #endregion
}