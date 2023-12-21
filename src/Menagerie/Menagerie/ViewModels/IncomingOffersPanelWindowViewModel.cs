using System.Collections.ObjectModel;
using Menagerie.Core;
using Menagerie.Models;
using Menagerie.Shared.Models.Trading;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOffersPanelWindowViewModel : ViewModelBase
{
    #region Props

    public ObservableCollection<IncomingOfferViewModel> Offers { get; set; } = [];
    private int _offersWidth = 50;

    #endregion

    #region Constructors

    public IncomingOffersPanelWindowViewModel()
    {
        Events.NewIncomingOffer += Events_OnNewIncomingOffer;
    }

    #endregion
    
    #region Public methods

    public void SetOffersWidth(int width)
    {
        _offersWidth = width;
    }

    #endregion

    #region Private methods

    private void Events_OnNewIncomingOffer(IncomingOffer offer)
    {
        Offers.Add(new IncomingOfferViewModel(new OfferModel(offer), _offersWidth));
        this.RaisePropertyChanged(nameof(Offers));
    }

    #endregion
}