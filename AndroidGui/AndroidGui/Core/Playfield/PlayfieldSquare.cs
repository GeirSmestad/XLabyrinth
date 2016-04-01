using AndroidGui.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndroidGui.Core.Playfield
{
    public class PlayfieldSquare
    {
        public int NumTreasures { get; set; }
        public SquareType Type { get; private set; }
        public Teleporter Hole { get; private set; }

        public PlayfieldSquare(SquareType type, int numTreasures, Teleporter hole=null)
        {
            Type = type;
            NumTreasures = numTreasures;
            Hole = hole;

            AssertThatSquareStateIsLegal();
        }

        private void AssertThatSquareStateIsLegal()
        {
            if (Type == SquareType.Teleporter && Hole == null)
            {
                throw new InvalidOperationException("A teleporter square needs to have a teleporter attached");
            }

            if (Type != SquareType.Teleporter && Hole != null)
            {
                throw new InvalidOperationException("A non-teleporter square cannot have a teleporter attached");
            }
        }
    }
}