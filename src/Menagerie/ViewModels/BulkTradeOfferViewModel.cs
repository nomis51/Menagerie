using System;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using Menagerie.Data.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class BulkTradeOfferViewModel : ReactiveObject
{
    #region Constants

    private const int MaxNameLength = 9;
    #endregion
    
    #region Members

    private readonly BulkTradeItemDto _bulkTradeItem;

    #endregion

    #region Props

    public bool Whispered { get; private set; }

    public string PayCurrency => _bulkTradeItem.PayCurrency;
    public string PayAmount => $"{Math.Round(_bulkTradeItem.PayAmount, 1)}x";
    public Uri PayCurrentImage => _bulkTradeItem.PayCurrencyImage;

    public string GetCurrency => _bulkTradeItem.GetCurrency;
    public string GetAmount => $"{Math.Round(_bulkTradeItem.GetAmount, 1)}x";
    public Uri GetCurrentImage => _bulkTradeItem.GetCurrencyImage;

    private string Player => _bulkTradeItem.Player.Length > MaxNameLength ? $"{_bulkTradeItem.Player[..MaxNameLength]}..." : _bulkTradeItem.Player;
    private string LastCharacterName => _bulkTradeItem.LastCharacterName.Length > MaxNameLength ? $"{_bulkTradeItem.LastCharacterName[..MaxNameLength]}..." : _bulkTradeItem.LastCharacterName;

    public string PlayerNameDisplay => $"{LastCharacterName} ({Player})";

    #endregion

    #region Constructors

    public BulkTradeOfferViewModel(BulkTradeItemDto bulkTradeItem)
    {
        _bulkTradeItem = bulkTradeItem;
    }

    #endregion

    #region Public methods

    public void SendWhisper()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.InjectToClipboard(_bulkTradeItem.Whisper);
        Whispered = true;
    }

    public void WhisperPlayer()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.PrepareToSendWhisper(_bulkTradeItem.LastCharacterName);
    }

    #endregion
}