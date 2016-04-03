using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Playfield
{
    public class PlayfieldSquare
    {
        public int NumTreasures { get; set; }
        public SquareType Type { get; private set; }
        public Teleporter Hole { get; private set; }
        public int X { get; set; }
        public int Y { get; set; }


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
                throw new LabyrinthInvalidStateException(
                    "A teleporter square needs to have a teleporter attached");
            }

            if (Type != SquareType.Teleporter && Hole != null)
            {
                throw new LabyrinthInvalidStateException(
                    "A non-teleporter square cannot have a teleporter attached");
            }
        }
    }
}