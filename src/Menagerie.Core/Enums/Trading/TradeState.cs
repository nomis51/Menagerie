namespace Menagerie.Core.Enums.Trading;

[Flags]
public enum TradeState
{
    Initial = 1,
    PlayerInvited = 2,
    HideoutJoined = 4,
    Trading = 8,
    Done = 16,
    Busy = 32,
    StillInterested = 64,
    ReSend = 128,
}