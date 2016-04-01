using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AndroidGui.Core.Moves
{
    enum MoveType
    {
        DoNothing,
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