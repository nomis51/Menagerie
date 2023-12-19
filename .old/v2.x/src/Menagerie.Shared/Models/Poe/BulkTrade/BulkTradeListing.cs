using Newtonsoft.Json;

namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeListing
{
    [JsonProperty("indexed")] public DateTime IndexedAt { get; set; }
    public BulkTradeListingAccount Account { get; set; }
    public List<BulkTradeOffer> Offers { get; set; } = new();
    public string Whisper { get; set; }

    #region Public methods

    public double CalculateHaveExchange(int haveCount)
    {
        var offer = Offers.FirstOrDefault();
        if (offer is null) return 0;

        return (haveCount * offer.Item.Amount) / offer.Exchange.Amount;
    }

    public double CalculateWantExchange(int wantCount)
    {
        var offer = Offers.FirstOrDefault();
        if (offer is null) return 0;

        return (wantCount * offer.Exchange.Amount) / offer.Item.Amount;
    }

    #endregion
}