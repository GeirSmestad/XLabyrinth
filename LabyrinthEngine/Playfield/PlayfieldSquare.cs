using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Playfield
{
    [Serializable]
    public class PlayfieldSquare : IEquatable<object>
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

        public override bool Equals(object item)
        {
            var other = item as PlayfieldSquare;

            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var result = true;

            result &= NumTreasures == other.NumTreasures;
            result &= Type == other.Type && X == other.X && Y == other.Y;
            
            if (Hole == null)
            {
               return result && other.Hole == null;
            }
            if (other.Hole == null)
            {
                return false;
            }
            result &= Hole.Equals(other.Hole);

            return result;
            
        }
    }
}