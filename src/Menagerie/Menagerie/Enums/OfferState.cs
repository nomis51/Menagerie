using System;

namespace Menagerie.Enums;

[Flags]
public enum OfferState
{
    Initial = 1,
    Busy = 2,
    StillInterested = 4,
    PlayerInvited = 8,
    PlayerJoined = 16,
    Trading = 32,
    Done = 64
}