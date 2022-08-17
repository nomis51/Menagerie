namespace Menagerie.Shared.Models.Poe.BulkTrade;

public class BulkTradeListingAccount
{
    public string Name { get; set; }
    public string LastCharacterName { get; set; }
    public BulkTradeListingAccountOnline Online { get; set; }
    public string Language { get; set; }
    public string Realm { get; set; }
}

public class BulkTradeListingAccountOnline {
public string League { get; set; }
}