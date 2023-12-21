using Menagerie.Shared.Models.Trading;

namespace Menagerie.Core;

public class Events
{
    #region ShowOverlay

    public delegate void ShowOverlayEvent();

    public static event ShowOverlayEvent? ShowOverlay;
    public static void ShowOverlayEventInvoke() => ShowOverlay?.Invoke();

    #endregion

    #region HideOverlay

    public delegate void HideOverlayEvent();

    public static event HideOverlayEvent? HideOverlay;
    public static void HideOverlayEventInvoke() => HideOverlay?.Invoke();

    #endregion

    #region TradeAccepted

    public delegate void TradeAcceptedEvent();

    public static event TradeAcceptedEvent? TradeAccepted;
    public static void TradeAcceptedEventInvoke() => TradeAccepted?.Invoke();

    #endregion

    #region TradeCancelled

    public delegate void TradeCancelledEvent();

    public static event TradeCancelledEvent? TradeCancelled;
    public static void TradeCancelledEventInvoke() => TradeCancelled?.Invoke();

    #endregion

    #region PlayerJoined

    public delegate void PlayerJoinedEvent(string player);

    public static event PlayerJoinedEvent? PlayerJoined;
    public static void PlayerJoinedEventInvoke(string player) => PlayerJoined?.Invoke(player);

    #endregion

    #region NewIncomingOffer

    public delegate void NewIncomingOfferEvent(IncomingOffer offer);

    public static event NewIncomingOfferEvent? NewIncomingOffer;
    public static void NewIncomingOfferEventInvoke(IncomingOffer offer) => NewIncomingOffer?.Invoke(offer);

    #endregion

    #region NewOutgoingOffer

    public delegate void NewOutgoingOfferEvent(OutgoingOffer offer);

    public static event NewOutgoingOfferEvent? NewOutgoingOffer;
    public static void NewOutgoingOfferEventInvoke(OutgoingOffer offer) => NewOutgoingOffer?.Invoke(offer);

    #endregion

    #region LocationUpdated

    public delegate void LocationUpdatedEvent(string location);

    public static event LocationUpdatedEvent? LocationUpdated;
    public static void LocationUpdatedEventInvoke(string location) => LocationUpdated?.Invoke(location);

    #endregion
}