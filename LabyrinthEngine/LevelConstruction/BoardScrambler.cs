﻿using LabyrinthEngine.LevelConstruction.BoardTransposition;
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
            return new BoardState(playfieldGrid, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
        }

        private void modifyLocalWorkingCopyAccordingTo(BoardTranspositionOperation operation)
        {
            // TODO: Can extract each method out to its own helper, for readability.
            // TODO: Rotation of 90 or 270 degrees will fail for non-quadratic boards.
            // Perhaps all that is required in this case is to re-initialize the target data structure
            // with the correct dimensions; the iteration should work nicely with no changes.
            var playfieldOriginal = HelperMethods.DeepClone(playfieldGrid);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var transposedCoords = transposeXYCoordinates(x, y, operation);
                    var squareToTranspose = playfieldOriginal[transposedCoords.X, transposedCoords.Y];
                    playfieldGrid[x, y] = new PlayfieldSquare(squareToTranspose.Type,
                        squareToTranspose.NumTreasures, squareToTranspose.Hole);
                }
            }

            var horizontalWallsOriginal = HelperMethods.DeepClone(horizontalWalls);
            for (int x = 0; x < width; x++)
            {
                for (int w_y = 0; w_y < height + 1; w_y++)
                {
                    var transposedCoords = transposeHorizontalWallCoordinates(x, w_y, operation);
                    var wallToTranspose = horizontalWallsOriginal[transposedCoords.X, transposedCoords.W_y];
                    horizontalWalls[x, w_y] = new WallSection(wallToTranspose.IsPassable,
                        wallToTranspose.HasHamster, wallToTranspose.IsExit, wallToTranspose.IsExterior);
                }
            }

            var verticalWallsOriginal = HelperMethods.DeepClone(verticalWalls);
            for (int y = 0; y < height; y++)
            {
                for (int w_x = 0; w_x < height + 1; w_x++)
                {
                    var transposedCoords = transposeVerticalWallCoordinates(y, w_x, operation);
                    var wallToTranspose = verticalWallsOriginal[transposedCoords.Y, transposedCoords.W_x];
                    horizontalWalls[y, w_x] = new WallSection(wallToTranspose.IsPassable,
                        wallToTranspose.HasHamster, wallToTranspose.IsExit, wallToTranspose.IsExterior);
                }
            }

            for (int i = 0; i < holes.Count; i++)
            {
                var transposedCoords = transposeXYCoordinates(holes[i].X, holes[i].Y, operation);
                holes[i] = new Teleporter(holes[i].TeleporterIndex, holes[i].NextHole,
                    transposedCoords.X, transposedCoords.Y);
            }

            var centaurOriginal = HelperMethods.DeepClone(centaur);
            var transposedStartCoords = transposeXYCoordinates(centaur.X, centaur.Y, operation);
            var transposedCentaurPath = new List<CentaurStep>();
            for (int i = 0; i < centaur.Path.Count; i++)
            {
                var transposedCoords = transposeXYCoordinates(
                    centaur.Path[i].X, centaur.Path[i].Y, operation);
                transposedCentaurPath.Add(new CentaurStep(transposedCoords.X, transposedCoords.Y, 
                    centaur.Path[i].IgnoreWallsWhenSteppingHere));
            }
            centaur = new Centaur(transposedStartCoords.X, transposedStartCoords.Y, transposedCentaurPath);

            for (int i = 0; i < startingPositions.Count; i++)
            {
                var transposedCoords = transposeXYCoordinates(startingPositions[i].X,
                    startingPositions[i].Y, operation);
                startingPositions[i] = new Position(transposedCoords.X, transposedCoords.Y);
            }
        }

        private void rotateBoardRight(int howMany90DegreesToRotate)
        {
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
