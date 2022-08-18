namespace Menagerie.Application.DTOs;

public class BulkTradeItemDto
{
    public double PayAmount { get; set; }
    public string PayCurrency { get; set; }
    public Uri PayCurrencyImage { get; set; }
    public string PayNativeWhisperTemplate { get; set; }

    public double GetAmount { get; set; }
    public string GetCurrency { get; set; }
    public Uri GetCurrencyImage { get; set; }
    public string GetNativeWhisperTemplate { get; set; }

    public string WhisperTemplate { get; set; }
    public string Player { get; set; }
    public string LastCharacterName { get; set; }

    public string Whisper => string.Format(WhisperTemplate, string.Format(PayNativeWhisperTemplate, PayAmount), string.Format(GetNativeWhisperTemplate, GetAmount));
}