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
using AndroidGui.Core.Entities;

namespace AndroidGui.Core.Playfield
{
    /// <summary>
    /// An instance of the Board class represents the current state of the
    /// play area.
    /// </summary>
    public class Board
    {
        public WallSection[,] WallGrid;
        public PlayfieldSquare[,] Playfield;
        public List<Teleporter> Holes;
        public Centaur centaur;
    }
}