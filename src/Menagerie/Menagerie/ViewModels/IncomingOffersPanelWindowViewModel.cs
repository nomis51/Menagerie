using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
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

    public void RemoveOffer(string id)
    {
        var index = Offers.Select(o => o.Offer.Id).IndexOf(id);
        if (index == -1) return;

        Offers[index].Removed -= RemoveOffer;
        Offers.RemoveAt(index);
        this.RaisePropertyChanged(nameof(Offers));
    }

    public void SetOffersWidth(int width)
    {
        _offersWidth = width;
    }

    #endregion

    #region Private methods

    private void Events_OnNewIncomingOffer(IncomingOffer offer)
    {
        var vm = new IncomingOfferViewModel(new OfferModel(offer), _offersWidth);
        Offers.Add(vm);
        vm.Removed += RemoveOffer;

        this.RaisePropertyChanged(nameof(Offers));
    }

    #endregion
}