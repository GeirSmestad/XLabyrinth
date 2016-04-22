using LabyrinthEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.LevelConstruction.BoardTransposition
{
    public class BoardRotation : BoardTranspositionOperation
    { 
        private int howMany90DegreesToRotateRight;

        public int HowMany90DegreesToRotateRight
        {
            get { return howMany90DegreesToRotateRight; }
            set
            {
                if (value == 0 || value == 1 || value == 2 || value == 3)
                {
                    howMany90DegreesToRotateRight = value;
                }
                else
                {
                    throw new LabyrinthInvalidStateException(
                        "HowMany90DegreesToRotateRight must be in [0,3].");
                }
            }
        }
    }
}
