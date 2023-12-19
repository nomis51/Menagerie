using System;
using System.Collections.ObjectModel;
using System.Reflection;
using DynamicData;
using Menagerie.Helpers;
using Menagerie.Models;

namespace Menagerie.ViewModels;

public class IncomingOffersPanelWindowViewModel : ViewModelBase
{
    #region Props

    public ObservableCollection<IncomingOfferViewModel> Offers { get; set; } = [];
    private int _offersWidth = 50;

    #endregion

    #region Public methods

    public void SetOffersWidth(int width)
    {
        _offersWidth = width;

        var random = new Random();
        for (var i = 0; i < 12; ++i)
        {
            Offers.Add(
                new IncomingOfferViewModel(new OfferModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Item = "Mageblood Heavy Belt",
                    Quantity = 0,
                    League = "Affliction",
                    Player = "BobRoss4269",
                    Time = DateTime.Now,
                    Price = new PriceModel
                    {
                        Currency = "Divine Orb",
                        Quantity = random.Next(1, 999),
                        CurrencyImage = ImageHelper.LoadFromWeb(
                            new Uri(
                                "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQ3VycmVuY3kvQ3VycmVuY3lNb2RWYWx1ZXMiLCJzY2FsZSI6MX1d/ec48896769/CurrencyModValues.png",
                                UriKind.Absolute)
                        ).Result!
                    },
                    Location = new ItemLocationModel
                    {
                        StashTab = "Uniques",
                        Left = 2,
                        Top = 5
                    }
                }, _offersWidth)
            );
        }
    }

    #endregion
}