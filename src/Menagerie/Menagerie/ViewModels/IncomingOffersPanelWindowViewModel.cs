using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using DynamicData;
using Menagerie.Core;
using Menagerie.Core.Services;
using Menagerie.Enums;
using Menagerie.Models;
using Menagerie.Shared.Models.Trading;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOffersPanelWindowViewModel : ViewModelBase
{
    #region Props

    public ObservableCollection<IncomingOfferViewModel> Offers { get; set; } = [];
    private int _offerSize = 99;

    #endregion

    #region Constructors

    public IncomingOffersPanelWindowViewModel()
    {
        Events.NewIncomingOffer += Events_OnNewIncomingOffer;
    }

    #endregion

    #region Public methods

    public void SetOfferSize(int size)
    {
        _offerSize = size;
    }

    public void RemoveOffer(string id)
    {
        var index = Offers.Select(o => o.Offer.Id).IndexOf(id);
        if (index == -1) return;

        Offers[index].Removed -= RemoveOffer;
        Offers.RemoveAt(index);
        this.RaisePropertyChanged(nameof(Offers));
    }

    public void RemoveAllOffers()
    {
        foreach (var vm in Offers)
        {
            if (!vm.Offer.State.HasFlag(OfferState.PlayerInvited)) continue;

            AppService.Instance.SendKickCommand(vm.Offer.Player);
        }

        Offers.Clear();
        this.RaisePropertyChanged(nameof(Offers));
    }

    #endregion

    #region Private methods

    private void Events_OnNewIncomingOffer(IncomingOffer offer)
    {
        var vm = new IncomingOfferViewModel(new OfferModel(offer), _offerSize);
        Offers.Add(vm);
        vm.Removed += RemoveOffer;

        this.RaisePropertyChanged(nameof(Offers));
    }

    #endregion
}