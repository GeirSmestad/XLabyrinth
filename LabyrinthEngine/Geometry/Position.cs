using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Geometry
{
    [Serializable]
    public class Position
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object item)
        {
            var other = item as Position;

            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ((X.Equals(other.X)) && (Y.Equals(other.Y)));
        }
        

        public override int GetHashCode()
        {
            return X.GetHashCode() * Y.GetHashCode();
        }
    }
}
