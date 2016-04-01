using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndroidGui.Core.Entities;

namespace AndroidGui.Core.Playfield
{
    /// <summary>
    /// An instance of the Board class represents the current state of the
    /// play area.
    /// </summary>
    public class BoardState
    {
        public WallSection[,] WallGrid;
        public PlayfieldSquare[,] PlayfieldGrid;
        public List<Teleporter> Holes;
        public Centaur centaur;
    }
}