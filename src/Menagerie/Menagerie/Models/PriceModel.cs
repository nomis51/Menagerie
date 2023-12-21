using System;
using Avalonia.Media.Imaging;
using Menagerie.Helpers;

namespace Menagerie.Models;

public class PriceModel
{
    public Bitmap CurrencyImage { get; set; }
    public string Currency { get; set; }
    public double Quantity { get; set; }

    public PriceModel(double quantity, string currency, Uri currencyImageUrl)
    {
        Quantity = quantity;
        Currency = currency;
        CurrencyImage = ImageHelper.LoadFromWeb(currencyImageUrl).Result!;
    }
}