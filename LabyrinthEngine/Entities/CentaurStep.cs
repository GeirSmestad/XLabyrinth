using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Entities
{
    [Serializable]
    public class CentaurStep
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public bool IgnoreWallsWhenSteppingHere { get; private set; }

        public CentaurStep(int x, int y, bool ignoreWallsWhenSteppingHere)
        {
            X = x;
            Y = y;
            this.IgnoreWallsWhenSteppingHere = ignoreWallsWhenSteppingHere;
        }
}
}
