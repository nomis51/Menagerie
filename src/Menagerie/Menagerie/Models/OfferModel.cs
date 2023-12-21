using System;
using System.Xml;
using Menagerie.Enums;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Models;

public class OfferModel
{
    public string Id { get; set; }
    public DateTime Time { get; set; }
    public string Player { get; set; }
    public string Item { get; set; }
    public int Quantity { get; set; }
    public PriceModel Price { get; set; }
    public string League { get; set; }
    public ItemLocationModel Location { get; set; }
    public OfferState State { get; set; } = OfferState.Initial;

    public OfferModel(Offer offer)
    {
        Id = offer.Id;
        Time = offer.Time;
        Player = offer.Player;
        Item = offer.ItemName;
        Quantity = 0;
        Price = new PriceModel(offer.Price, offer.Currency, offer.CurrencyImageUri);
        League = offer.League;
        Location = new ItemLocationModel(offer.StashTab, offer.Left, offer.Top);
    }
}