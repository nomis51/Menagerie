namespace Menagerie.Shared.Models.PoeNinja
{
    public class PoeNinjaCurrency
    {
        public double ChaosEquivalent { get; set; }
        public string CurrencyTypeName { get; set; }
        public PoeNinjaExchange Pay { get; set; }
        public PoeNinjaExchange Receive { get; set; }
        public PoeNinjaSparkLine PaySparkLine { get; set; }
        public PoeNinjaSparkLine ReceiveSparkLine { get; set; }
        public PoeNinjaSparkLine LowConfidencePaySparkLine { get; set; }
        public PoeNinjaSparkLine LowConfidenceReviceSparkLine { get; set; }
        public string DetailsId { get; set; }
    }
}