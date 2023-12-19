using System;

namespace Menagerie.Enums;

[Flags]
public enum OfferState
{
    Initial,
    Busy,
    StillInterested,
    PlayerInvited,
    PlayerJoined,
    Trading,
    Done
}