using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine
{
    /// <summary>
    /// Since humans are good at remembering spatial information, this class allows you to rotate
    /// a particular board in order to make it less likely that the operator remembers which board
    /// has been selected. The internal geometry of the board remains unmodified.
    /// </summary>
    /// <param name="howMany90DegreesToRotateRight">Should be 0, 1, 2 or 3.</param>
    public class BoardScrambler
    {
        // TODO: Add parameters for each element of the original.
        // Can maybe delete the "original" parameter after doing this.
        private WallSection[,] horizontalWalls;
        private WallSection[,] verticalWalls;
        private PlayfieldSquare[,] playfieldGrid;
        private List<Teleporter> holes;
        private Centaur centaur;
        private List<Position> startingPositions;

        private PlayfieldAxis axisToRotateBoardAbout;

        public BoardScrambler(BoardState original,
            int howMany90DegreesToRotateRight,
            bool flipAlongHorizontalAxis = false,
            bool flipAlongVerticalAxis = false,
            bool scrambleTeleporterOrder = false,
            bool scrambleTreasureLocations = false,
            bool scrambleStartingPositions = false
            )
        {
            var workingCopy = HelperMethods.DeepClone(original);
            horizontalWalls = workingCopy.HorizontalWalls;
            verticalWalls = workingCopy.VerticalWalls;
            playfieldGrid = workingCopy.PlayfieldGrid;
            holes = workingCopy.Holes;
            centaur = workingCopy.centaur;
            startingPositions = workingCopy.StartingPositions;
            
            rotateRight(howMany90DegreesToRotateRight);
            if (flipAlongHorizontalAxis) { flipBoardAlongHorizontalAxis(); }
            if (flipAlongVerticalAxis) { flipBoardAlongVerticalAxis(); }
            if (scrambleTeleporterOrder) { scrambleBoardTeleporterOrder(); }
            if (scrambleTreasureLocations) { scrambleBoardTreasureLocations(); }
            if (scrambleStartingPositions) { scrambleBoardStartingPositions(); }
        }

        public BoardState ReturnScrambledBoard()
        {
            throw new NotImplementedException();
        }

        private void copyOriginalContentsToLocalWorkingCopy()
        {
            throw new NotImplementedException();
        }

        private void rotateRight(int howMany90DegreesToRotate)
        {
            // Simplest solution involves methods that map each (x,y), (x, w_y), (y, w_x) coordinate
            // to the rotated form, and then run through the board to rotate it. Should also
            // have an option for scrambling teleporter ordering and treasure locations.
            throw new NotImplementedException();
        }

        private void flipBoardAlongHorizontalAxis()
        {
            throw new NotImplementedException();
        }
        private void flipBoardAlongVerticalAxis()
        {
            throw new NotImplementedException();
        }

        private void scrambleBoardTreasureLocations()
        {
            throw new NotImplementedException();
        }

        private void scrambleBoardTeleporterOrder()
        {
            throw new NotImplementedException();
        }

        private void scrambleBoardStartingPositions()
        {
            throw new NotImplementedException();
        }

        private Position rotateXYCoordinatesRight(int x, int y)
        {
            throw new NotImplementedException();
        }

        private HorizontalWallCoordinate rotateHorizontalWallCoordinatesRight(int x, int w_y)
        {
            throw new NotImplementedException();
        }

        private VerticalWallCoordinate rotateVerticalWallCoordinates(int y, int w_x)
        {
            throw new NotImplementedException();
        }

        private Position flipXYCoordinates(int x, int y)
        {
            throw new NotImplementedException();
        }

        private HorizontalWallCoordinate flipHorizontalWallCoordinates(int x, int w_y)
        {
            throw new NotImplementedException();
        }

        private VerticalWallCoordinate flipVerticalWallCoordinates(int y, int w_x)
        {
            throw new NotImplementedException();
        }
    }
}
