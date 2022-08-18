namespace Menagerie.Application.DTOs;

public class BulkTradeItemDto
{
    public double PayAmount { get; set; }
    public string PayCurrency { get; set; }
    public Uri PayCurrencyImage { get; set; }
    
    public double GetAmount { get; set; }
    public string GetCurrency { get; set; }
    public Uri GetCurrencyImage { get; set; }
    
    public string Whisper { get; set; }
    public string Player { get; set; }
}