using System;
using Menagerie.Application.DTOs;
using Menagerie.Application.Services;
using Menagerie.Data.Services;
using ReactiveUI;

namespace Menagerie.ViewModels;

public class BulkTradeOfferViewModel : ReactiveObject
{
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

    public string Player => _bulkTradeItem.Player.Length > 15 ? _bulkTradeItem.Player[..15] : _bulkTradeItem.Player;

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
        AppService.Instance.SendChatMessage(_bulkTradeItem.Whisper);
        AppService.Instance.EnsureOverlayFocused();
        Whispered = true;
    }

    public void WhisperPlayer()
    {
        AppService.Instance.PlayClickSoundEffect();
        AppService.Instance.PrepareToSendWhisper(_bulkTradeItem.Player);
    }

    #endregion
}