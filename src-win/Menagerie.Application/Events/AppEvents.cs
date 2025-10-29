using Menagerie.Application.DTOs;

namespace Menagerie.Application.Events;

public class AppEvents
{
    #region New incoming offer

    public delegate void NewIncomingOfferEvent(IncomingOfferDto offer);

    public static event NewIncomingOfferEvent OnNewIncomingOffer;

    public static void NewIncomingOfferEventInvoke(IncomingOfferDto offer) => OnNewIncomingOffer?.Invoke(offer);

    #endregion
    
    #region New outgoing offer

    public delegate void NewOutgoingOfferEvent(OutgoingOfferDto offer);

    public static event NewOutgoingOfferEvent OnNewOutgoingOffer;

    public static void NewOutgoingOfferEventInvoke(OutgoingOfferDto offer) => OnNewOutgoingOffer?.Invoke(offer);

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

    public delegate void NewChaosRecipeEvent(List<ChaosRecipeItemDto> items);

    public static event NewChaosRecipeEvent OnNewChaosRecipe;

    public static void NewChaosRecipeEventInvoke(List<ChaosRecipeItemDto> items) => OnNewChaosRecipe?.Invoke(items);

    #endregion
    
    #region Change overlay visibilty

    public delegate void OverlayVisibilityChangeEvent(bool isVisible);

    public static event OverlayVisibilityChangeEvent OnOverlayVisibilityChange;

    public static void OverlayVisibilityChangeEventInvoke(bool isVisible) =>
        OnOverlayVisibilityChange?.Invoke(isVisible);

    #endregion
    
    #region Chat scan message

    public delegate void ChatMessageFoundEvent(ChatMessageDto chatMessage);

    public static event ChatMessageFoundEvent OnChatMessageFound;

    public static void ChatMessageFoundEventInvoke(ChatMessageDto chatMessage) =>
        OnChatMessageFound?.Invoke(chatMessage);

    #endregion
    
    #region Scam detected

    public delegate void ScamDetectedEvent(string id, string price);

    public static event ScamDetectedEvent OnScamDetected;

    public static void ScamDetectedEventInvoke(string id, string price) => OnScamDetected?.Invoke(id, price);

    #endregion
    
    #region Highlight item

    public delegate void HighlightItemEvent(bool isVisible, int left, int top, int leftSize, int topSize, string stashTab);

    public static event HighlightItemEvent OnHighlightItem;

    public static void HighlightItemEventInvoke(bool isVisible, int left, int top, int leftSize, int topSize, string stashTab) => OnHighlightItem?.Invoke(isVisible, left, top, leftSize, topSize, stashTab);

    #endregion
    
    #region Search outgoing offer

    public delegate void SearchOutgoingOfferEvent();

    public static event SearchOutgoingOfferEvent OnSearchOutgoingOffer;

    public static void SearchOutgoingOfferEventInvoke() =>
        OnSearchOutgoingOffer?.Invoke();

    #endregion
}