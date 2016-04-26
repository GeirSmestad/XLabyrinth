using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Geometry
{
    public class VerticalWallCoordinate
    {
        public VerticalWallCoordinate(int y, int w_x)
        {
            Y = y;
            W_x = w_x;
        }

        public int Y { get; private set; }
        public int W_x { get; private set; }
    }
}
