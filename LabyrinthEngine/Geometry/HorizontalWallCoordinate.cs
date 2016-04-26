using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Geometry
{
    public class HorizontalWallCoordinate
    {
        public HorizontalWallCoordinate(int x, int w_y)
        {
            X = x;
            W_y = w_y;
        }

        public int X { get; private set; }
        public int W_y { get; private set; }
    }
}
