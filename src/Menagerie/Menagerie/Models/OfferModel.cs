using System;

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
}