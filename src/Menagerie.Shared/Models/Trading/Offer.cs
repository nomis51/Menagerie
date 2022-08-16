namespace Menagerie.Shared.Models.Trading;

public class Offer : Entity
{
    public DateTime Time { get; set; }
    public string ItemName { get; set; }
    public string Player { get; set; }
    public string League { get; set; }
    public Uri CurrencyImageUri { get; set; }
    public string PriceStr { get; set; }
    public PriceConversions PriceConversions { get; set; }
    public string Whisper { get; set; }
    public string StashTab { get; set; }
    public string LeftStr { get; set; }
    public string TopStr { get; set; }
    public int Left => int.TryParse(LeftStr, out var intValue) ? intValue : 0;
    public int Top => int.TryParse(TopStr, out var intValue) ? intValue : 0;
    public int Width { get; set; }
    public int Height { get; set; }

    public double Price
    {
        get
        {
            try
            {
                if (string.IsNullOrEmpty(PriceStr)) return 0.0d;

                var splits = PriceStr.Split(" ");
                if (splits.Length < 1) return 0;
                if (!double.TryParse(splits[0], out var value)) return 0;
                return Math.Round(value, 1);
            }
            catch (Exception)
            {
                return 0.0d;
            }
        }
    }

    public string Currency
    {
        get
        {
            if (PriceStr is null) return string.Empty;
            var splits = PriceStr.Split(" ");
            return splits.Length < 2 ? string.Empty : string.Join(" ", splits.Skip(1));
        }
    }

    public Offer()
    {
    }
}