using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Moves
{
    public enum MoveType
    {
        DoNothing,
        FallThroughHole,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight,
        FireUp,
        FireDown,
        FireLeft,
        FireRight,
        ThrowGrenadeUp,
        ThrowGrenadeDown,
        ThrowGrenadeLeft,
        ThrowGrenadeRight,
        PlaceHamsterUp,
        PlaceHamsterDown,
        PlaceHamsterLeft,
        PlaceHamsterRight,
        HamsterSprayUp,
        HamsterSprayDown,
        HamsterSprayLeft,
        HamsterSprayRight,
        BuildWallUp,
        BuildWallDown,
        BuildWallLeft,
        BuildWallRight
    }
}