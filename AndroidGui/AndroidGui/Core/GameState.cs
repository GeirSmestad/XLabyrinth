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
using AndroidGui.Core.Actors;
using AndroidGui.Core.Entities;

namespace AndroidGui.Core
{
    class GameState
    {
        WallSection[,] WallGrid;
        PlayfieldSquare[,] Playfield;
        List<Teleporter> Holes;
        Centaur centaur;

        List<Player> Players;
        List<Action> CompletedMoves;

        Random randomNumberGenerator;
        int initialRngSeed; // Must store this to facilitate rebuilding game state from move list




    }
}