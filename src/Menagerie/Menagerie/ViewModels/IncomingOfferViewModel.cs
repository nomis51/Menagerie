using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using Menagerie.Models;

namespace Menagerie.ViewModels;

public class IncomingOfferViewModel : ViewModelBase
{
    #region Props

    public OfferModel Offer { get; set; }
    public int Width { get; set; }

    public int PriceQuantityFontSize => Offer.Price.Quantity switch
    {
        < 10 => 22,
        < 100 => 20,
        < 1000 => 16,
        < 10000 => 13,
        < 100000 => 12,
        _ => 0
    };

    public ITransform? PriceQuantityTransform => Offer.Price.Quantity >= 10000 ? TransformOperations.Parse("rotate(-35deg)") : null;
    public ObservableCollection<Tuple<string, string>> TooltipLines { get; private set; } = [];

    #endregion

    #region Constructors

    public IncomingOfferViewModel(OfferModel offer, int width = 50)
    {
        Offer = offer;
        Width = width;

        TooltipLines.Add(new Tuple<string, string>("Time : ", Offer.Time.ToLongTimeString()));
        TooltipLines.Add(new Tuple<string, string>("Player : ", Offer.Player));
        TooltipLines.Add(new Tuple<string, string>("Item : ", $"{(Offer.Quantity > 0 ? $"{Offer.Quantity}x " : string.Empty)}{Offer.Item}"));
        TooltipLines.Add(new Tuple<string, string>("Price : ", $"{Offer.Price.Quantity} {Offer.Price.Currency}"));
        TooltipLines.Add(new Tuple<string, string>("League : ", Offer.League));
        TooltipLines.Add(new Tuple<string, string>("Location : ", $"{Offer.Location.StashTab} (Left: {Offer.Location.Left}, Top: {Offer.Location.Top})"));
    }

    #endregion
}