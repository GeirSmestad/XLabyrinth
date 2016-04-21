using LabyrinthEngine.LevelConstruction.BoardTransposition;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Geometry;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.LevelConstruction
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

        private int height;
        private int width;

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

            height = workingCopy.Height;
            width = workingCopy.Width;
            
            rotateBoardRight(howMany90DegreesToRotateRight);
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

        private void modifyLocalWorkingCopyAccordingTo(BoardTranspositionOperation operation)
        {
            // TODO: Run through all parts of the board, changing them as instructed.
            throw new NotImplementedException();
        }
 

        private void rotateBoardRight(int howMany90DegreesToRotate)
        {
            // Simplest solution involves methods that map each (x,y), (x, w_y), (y, w_x) coordinate
            // to the rotated form, and then run through the board to rotate it. Should also
            // have an option for scrambling teleporter ordering and treasure locations.
            modifyLocalWorkingCopyAccordingTo(new BoardRotation()
            { HowMany90DegreesToRotateRight = howMany90DegreesToRotate });
        }

        private void flipBoardAlongHorizontalAxis()
        {
            modifyLocalWorkingCopyAccordingTo(
                new BoardFlip() { AxisToFlipAbout = PlayfieldAxis.Horizontal});
        }
        private void flipBoardAlongVerticalAxis()
        {
            modifyLocalWorkingCopyAccordingTo(
                new BoardFlip() { AxisToFlipAbout = PlayfieldAxis.Vertical });
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

        private Position transposeXYCoordinates(int x, int y, 
            BoardTranspositionOperation transpositionToPerform)
        {
            if (transpositionToPerform.GetType() == typeof(BoardFlip))
            {
                var operation = (BoardFlip)transpositionToPerform;
                if (operation.AxisToFlipAbout == PlayfieldAxis.Horizontal)
                {
                    return new Position(x, height - y - 1);
                }
                else // if (operation.AxisToFlipAbout == PlayfieldAxis.Vertical)
                {
                    return new Position(width - x - 1, y);
                }
            }
            else if (transpositionToPerform.GetType() == typeof(BoardRotation))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new LabyrinthInvalidStateException(
                    "Coordinate transposition must have a legal type");
            }
        }

        private HorizontalWallCoordinate transposeHorizontalWallCoordinates(int x, int w_y, 
            BoardTranspositionOperation transpositionToPerform)
        {
            if (transpositionToPerform.GetType() == typeof(BoardFlip))
            {
                throw new NotImplementedException();
            }
            else if (transpositionToPerform.GetType() == typeof(BoardRotation))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new LabyrinthInvalidStateException(
                    "Coordinate transposition must have a legal type");
            }
        }

        private VerticalWallCoordinate transposeVerticalWallCoordinates(int y, int w_x,
            BoardTranspositionOperation transpositionToPerform)
        {
            if (transpositionToPerform.GetType() == typeof(BoardFlip))
            {
                throw new NotImplementedException();
            }
            else if (transpositionToPerform.GetType() == typeof(BoardRotation))
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new LabyrinthInvalidStateException(
                    "Coordinate transposition must have a legal type");
            }
        }
    }
}
