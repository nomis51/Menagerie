using Menagerie.Shared.Models.Chat;
using Menagerie.Shared.Models.Poe.Stash;
using Menagerie.Shared.Models.Trading;

namespace Menagerie.Data.Events;

public static class DataEvents
{
    #region New incoming offer

    public delegate void NewIncomingOfferEvent(IncomingOffer offer);

    public static event NewIncomingOfferEvent OnNewIncomingOffer;

    public static void NewIncomingOfferEventInvoke(IncomingOffer offer) => OnNewIncomingOffer?.Invoke(offer);

    #endregion

    #region New outgoing offer

    public delegate void NewOutgoingOfferEvent(OutgoingOffer offer);

    public static event NewOutgoingOfferEvent OnNewOutgoingOffer;

    public static void NewOutgoingOfferEventInvoke(OutgoingOffer offer) => OnNewOutgoingOffer?.Invoke(offer);

    #endregion

    #region Trade accepted

    public delegate void TradeAcceptedEvent();

    public static event TradeAcceptedEvent OnTradeAccepted;

    public static void TradeAcceptedEventInvoke() => OnTradeAccepted?.Invoke();

    #endregion

    #region Trade cancelled

    public delegate void TradeCancelledEvent();

    public static event TradeCancelledEvent OnTradeCancelled;

    public static void TradeCancelledEventInvoke() => OnTradeCancelled?.Invoke();

    #endregion

    #region Player joined

    public delegate void PlayerJoinedEvent(string player);

    public static event PlayerJoinedEvent OnPlayerJoined;

    public static void PlayerJoinedEventInvoke(string player) => OnPlayerJoined?.Invoke(player);

    #endregion

    #region New chaos recipe

    public delegate void NewChaosRecipeEvent(ChaosRecipe chaosRecipe);

    public static event NewChaosRecipeEvent OnNewChaosRecipe;

    public static void NewChaosRecipeEventInvoke(ChaosRecipe chaosRecipe) => OnNewChaosRecipe?.Invoke(chaosRecipe);

    #endregion

    #region Change overlay visibilty

    public delegate void OverlayVisibilityChangeEvent(bool isVisible);

    public static event OverlayVisibilityChangeEvent OnOverlayVisibilityChange;

    public static void OverlayVisibilityChangeEventInvoke(bool isVisible) =>
        OnOverlayVisibilityChange?.Invoke(isVisible);

    #endregion

    #region Chat scan message

    public delegate void ChatMessageFoundEvent(ChatMessage chatMessage);

    public static event ChatMessageFoundEvent OnChatMessageFound;

    public static void ChatMessageFoundEventInvoke(ChatMessage chatMessage) =>
        OnChatMessageFound?.Invoke(chatMessage);

    #endregion

    #region New whisper to send

    public delegate void NewWhisperToSendEvent(string whisper);

    public static event NewWhisperToSendEvent OnNewWhisperToSend;

    public static void NewWhisperToSendEventInvoke(string whisper) =>
        OnNewWhisperToSend?.Invoke(whisper);

    #endregion

    #region Location updated

    public delegate void LocationUpdatedEvent(string location);

    public static event LocationUpdatedEvent OnLocationUpdated;

    public static void LocationUpdatedEventInvoke(string location) =>
        OnLocationUpdated?.Invoke(location);

    #endregion

    #region Scam detected

    public delegate void ScamDetectedEvent(string id, string price);

    public static event ScamDetectedEvent OnScamDetected;

    public static void ScamDetectedEventInvoke(string id, string price) => OnScamDetected?.Invoke(id, price);

    #endregion

    #region Search outgoing offer

    public delegate void SearchOutgoingOfferEvent();

    public static event SearchOutgoingOfferEvent OnSearchOutgoingOffer;

    public static void SearchOutgoingOfferEventInvoke() =>
        OnSearchOutgoingOffer?.Invoke();

    #endregion

    #region App exit

    public delegate void ApplicationExitEvent();

    public static event ApplicationExitEvent OnApplicationExit;

    public static void ApplicationExitEventInvoke() =>
        OnApplicationExit?.Invoke();

    #endregion
}