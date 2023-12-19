using System;
using Avalonia.Media.Imaging;

namespace Menagerie.Models;

public class PriceModel
{
    public Bitmap CurrencyImage { get; set; }
    public string Currency { get; set; }
    public double Quantity { get; set; }
}