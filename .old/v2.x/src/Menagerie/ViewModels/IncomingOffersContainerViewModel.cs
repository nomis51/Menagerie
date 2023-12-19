using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class IncomingOffersContainerViewModel : ReactiveObject
{
    #region Members

    private readonly SourceList<IncomingOfferViewModel> _incomingOffers = new();

    #endregion

    #region Props

    public ReadOnlyObservableCollection<IncomingOfferViewModel> IncomingOffers;

    #endregion

    #region Constructors

    public IncomingOffersContainerViewModel()
    {
        _incomingOffers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out IncomingOffers)
            .Subscribe();

        AppEvents.OnNewIncomingOffer += AppEvents_OnNewIncomingOffer;
        AppEvents.OnTradeAccepted += AppEvents_OnTradeAccepted;
        AppEvents.OnTradeCancelled += AppEvents_OnTradeCancelled;
        AppEvents.OnPlayerJoined += AppEvents_OnPlayerJoined;
    }

    #endregion

    #region Public methods

    public void RemoveAllIncomingOffers()
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();

        if (_incomingOffers.Count < 8)
        {
            _incomingOffers.Clear();
        }
        else
        {
            _incomingOffers.RemoveRange(0, 8);
        }
    }

    #endregion

    #region Private methods

    private void HandlePlayerJoined(string player)
    {
        foreach (var offer in _incomingOffers.Items)
        {
            if (offer.Player != player) continue;

            offer.PlayerHasJoined();
        }
    }

    private void HandleTradeCancelled()
    {
        foreach (var offer in _incomingOffers.Items)
        {
            if (!offer.HasSentTradeRequest) continue;

            offer.CancelTradeRequest();
            break;
        }
    }

    private void HandleTradeAccepted()
    {
        foreach (var offerViewModel in _incomingOffers.Items)
        {
            if (!offerViewModel.HasSentTradeRequest) continue;

            var settings = AppService.Instance.GetSettings();

            if (settings.IncomingTrades.AutoKick)
            {
                AppService.Instance.SendKickCommand(offerViewModel.Player);
            }

            if (settings.IncomingTrades.AutoThanks)
            {
                AppService.Instance.SendThanksWhisper(offerViewModel.Player);
            }

            AppService.Instance.SaveTradeStatistic(offerViewModel.Offer);

            RemoveIncomingOffer(offerViewModel);
            break;
        }
    }

    private void RemoveIncomingOffer(IncomingOfferViewModel vm)
    {
        vm.OnOfferRemoved -= IncomingOfferViewModel_OnOfferRemoved;
        _incomingOffers.Remove(vm);
    }

    private void AddIncomingOffer(IncomingOfferDto offer)
    {
        var vm = new IncomingOfferViewModel(offer);
        vm.OnOfferRemoved += IncomingOfferViewModel_OnOfferRemoved;
        _incomingOffers.Add(vm);
    }

    private void IncomingOfferViewModel_OnOfferRemoved(IncomingOfferViewModel vm)
    {
        RemoveIncomingOffer(vm);
    }

    private void AppEvents_OnTradeCancelled()
    {
        HandleTradeCancelled();
    }

    private void AppEvents_OnTradeAccepted()
    {
        HandleTradeAccepted();
    }

    private void AppEvents_OnNewIncomingOffer(IncomingOfferDto offer)
    {
        AppService.Instance.PlayNewOfferSoundEffect();
        AddIncomingOffer(offer);
    }

    private void AppEvents_OnPlayerJoined(string player)
    {
        HandlePlayerJoined(player);
    }

    #endregion
}