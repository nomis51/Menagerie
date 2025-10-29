using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using Menagerie.Application.DTOs;
using Menagerie.Application.Events;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class OutgoingOffersContainerViewModel : ReactiveObject
{
    #region Events

    public delegate void FocusSearchOutgoingOfferEvent();

    public event FocusSearchOutgoingOfferEvent OnFocusSearchOutgoingOffer;

    #endregion

    #region Members

    private readonly Queue<OutgoingOfferDto> _outgoingOffersData = new();
    private readonly SourceList<OutgoingOfferViewModel> _outgoingOffers = new();
    private bool _isSearchOutgoingOfferVisible;

    #endregion

    #region Props

    public ReadOnlyObservableCollection<OutgoingOfferViewModel> OutgoingOffers;

    public bool IsSearchOutgoingOfferVisible
    {
        get => _isSearchOutgoingOfferVisible;
        private set => this.RaiseAndSetIfChanged(ref _isSearchOutgoingOfferVisible, value);
    }

    #endregion

    #region Constructors

    public OutgoingOffersContainerViewModel()
    {
        _outgoingOffers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out OutgoingOffers)
            .Subscribe();

        AppEvents.OnNewOutgoingOffer += AppEvents_OnNewOutgoingOffer;
        AppEvents.OnSearchOutgoingOffer += AppEvents_OnSearchOutgoingOffer;
    }

    #endregion

    #region Public methods

    public void ShowOutgoingOffers(string text)
    {
        var offers = _outgoingOffersData.Where(o => o.Player.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
            .ToList();

        _outgoingOffers.Clear();
        if (!offers.Any()) return;

        foreach (var vm in offers.Select(offer => new OutgoingOfferViewModel(offer)))
        {
            vm.OnOfferRemoved += OutgoingOfferViewModel_OnOfferRemoved;
            _outgoingOffers.Add(vm);
        }
    }

    public void ToggleSearchOutgoingOffer()
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();
        IsSearchOutgoingOfferVisible = !IsSearchOutgoingOfferVisible;
    }

    #endregion

    #region Private methods

    private void AppEvents_OnSearchOutgoingOffer()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(delegate
        {
            AppService.Instance.EnsureOverlayFocused();
            IsSearchOutgoingOfferVisible = !IsSearchOutgoingOfferVisible;
            OnFocusSearchOutgoingOffer?.Invoke();
        });
    }

    private void AppEvents_OnNewOutgoingOffer(OutgoingOfferDto offer)
    {
        AddOutgoingOffer(offer);
    }

    private void AddOutgoingOffer(OutgoingOfferDto offer)
    {
        _outgoingOffersData.Enqueue(offer);

        while (_outgoingOffersData.Count > 30)
        {
            _outgoingOffersData.Dequeue();
        }
    }

    private void OutgoingOfferViewModel_OnOfferRemoved(OutgoingOfferViewModel vm)
    {
        AppService.Instance.EnsureGameFocused();
        AppService.Instance.PlayClickSoundEffect();
        _outgoingOffers.Remove(vm);
    }

    #endregion
}