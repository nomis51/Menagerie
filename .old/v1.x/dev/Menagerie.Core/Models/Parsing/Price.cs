namespace Menagerie.Core.Models.Parsing
{
    public class Price
    {
        public double Value { get; set; }
        public string Currency { get; set; }
        public string NormalizedCurrency { get; set; }
        public string ImageLink { get; set; }
    }
}