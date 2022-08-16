namespace Menagerie.Models
{
    public class PricingResult
    {
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string CurrencyImageLink { get; set; }
        public string PlayerName { get; set; }

        public string PricingText
        {
            get { return $"{Price}x"; }
        }
    }
}