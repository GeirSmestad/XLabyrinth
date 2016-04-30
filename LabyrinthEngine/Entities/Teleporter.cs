using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Entities
{
    /// <summary>
    /// A hole, in Labyrinth terminology, is a teleporter. When you fall through a hole,
    /// you end up on top of the next hole awaiting your next turn. The holes are connected 
    /// in a loop.
    /// </summary>
    [Serializable]
    public class Teleporter : IEquatable<object>
    {
        public int TeleporterIndex { get; private set; }
        public Teleporter NextHole { get; set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public Teleporter(int teleporterIndex, Teleporter nextHole, int x, int y)
        {
            TeleporterIndex = teleporterIndex;
            NextHole = nextHole;
            X = x;
            Y = y;
        }

        public override bool Equals(object item)
        {
            var other = item as Teleporter;

            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var result = true;
            result &= X == other.X && Y == other.Y;
            result &= TeleporterIndex == other.TeleporterIndex;
            if (NextHole != null)
            {
                result &= other.NextHole != null;
                result &= NextHole.X == other.NextHole.X;
                result &= NextHole.Y == other.NextHole.Y;
                result &= NextHole.TeleporterIndex == other.NextHole.TeleporterIndex;
            }
            else
            {
                result &= other.NextHole == null;
            }

            return result;
        }
    }
}