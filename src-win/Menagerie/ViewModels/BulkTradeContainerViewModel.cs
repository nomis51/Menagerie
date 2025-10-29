using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Menagerie.Application.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class BulkTradeContainerViewModel : ReactiveObject
{
    #region Events

    #endregion

    #region Members

    private readonly SourceList<BulkTradeOfferViewModel> _bulkTradeOffers = new();
    private readonly SourceList<string> _currencies = new();
    private int _minGet = 1;
    private string _want;
    private string _have;
    private bool _isLoading;

    #endregion

    #region Props

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public ReadOnlyObservableCollection<BulkTradeOfferViewModel> BulkTradeOffers;
    public ReadOnlyObservableCollection<string> Currencies;

    public int MinGet
    {
        get => _minGet;
        set => this.RaiseAndSetIfChanged(ref _minGet, value);
    }

    public string Want
    {
        get => _want;
        set => this.RaiseAndSetIfChanged(ref _want, value);
    }

    public string Have
    {
        get => _have;
        set => this.RaiseAndSetIfChanged(ref _have, value);
    }

    #endregion

    #region Constructors

    public BulkTradeContainerViewModel()
    {
        _bulkTradeOffers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out BulkTradeOffers)
            .Subscribe();

        _currencies
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out Currencies)
            .Subscribe();

        RetrieveCurrencies();
    }

    #endregion

    #region Public methods

    public async Task Search()
    {
        if (MinGet < 0)
        {
            MinGet = 1;
        }

        if (string.IsNullOrEmpty(Have) || string.IsNullOrEmpty(Want)) return;

        IsLoading = true;
        var result = await AppService.Instance.SearchBulkTrade(Have, Want, MinGet);
        var vms = result.Select(r => new BulkTradeOfferViewModel(r));
        
        _ = Task.Run(() =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                _bulkTradeOffers.Clear();
                _bulkTradeOffers.AddRange(vms);
                IsLoading = false;
            });
        });
    }

    #endregion

    #region Private methods

    private void RetrieveCurrencies()
    {
        var result = AppService.Instance.GetCurrencies();
        _currencies.AddRange(result);
    }

    #endregion
}