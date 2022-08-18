using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using Menagerie.Application.DTOs;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class BulkTradeContainerViewModel : ReactiveObject
{
    #region Members

    private readonly SourceList<BulkTradeOfferViewModel> _bulkTradeOffers = new();

    #endregion

    #region Props

    public ReadOnlyObservableCollection<BulkTradeOfferViewModel> BulkTradeOffers;

    #endregion

    #region Constructors

    public BulkTradeContainerViewModel()
    {
        _bulkTradeOffers
            .Connect()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out BulkTradeOffers)
            .Subscribe();

        _bulkTradeOffers.AddRange(new ObservableCollection<BulkTradeOfferViewModel>(new []
        {
            new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }),
             new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }),
              new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            }), new BulkTradeOfferViewModel(new BulkTradeItemDto
            {
                PayAmount = 118,
                PayCurrency = "Chaos Orb",
                PayCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lSZXJvbGxSYXJlIiwic2NhbGUiOjF9XQ/46a2347805/CurrencyRerollRare.png", UriKind.Absolute),
                GetAmount = 1,
                GetCurrency = "Divine Orb",
                GetCurrencyImage = new Uri("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png", UriKind.Absolute),
                Player = "Player123",
                Whisper = "@Player123 Hi, I would like to buy your 5 Divine Orb listed for 590 Chaos Orb in Standard"
            })
        }));
    }

    #endregion
}