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
            playfieldGrid = workingCopy.PlayfieldGrid;
            holes = workingCopy.Holes;
            centaur = workingCopy.centaur;
            startingPositions = workingCopy.StartingPositions;
            horizontalWalls = workingCopy.HorizontalWalls;
            verticalWalls = workingCopy.VerticalWalls;

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
            // Unsure if this is the right way to swap width/height.
            //if (IsRotationOf90Or270Degrees(operation))
            //{
            //    FlipDimensionsOfWorkingCopy();
            //}

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
                    var squareToTranspose = playfieldOriginal[x, y];
                    playfieldGrid[transposedCoords.X, transposedCoords.Y] =
                        new PlayfieldSquare(squareToTranspose.Type,
                        squareToTranspose.NumTreasures, squareToTranspose.Hole);
                }
            }

            var horizontalWallsOriginal = HelperMethods.DeepClone(horizontalWalls);
            var verticalWallsOriginal = HelperMethods.DeepClone(verticalWalls);
            if (operation.GetType() == typeof(BoardFlip))
            {
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
            
                for (int y = 0; y < height; y++)
                {
                    for (int w_x = 0; w_x < height + 1; w_x++)
                    {
                        var transposedCoords = transposeVerticalWallCoordinates(y, w_x, operation);
                        var wallToTranspose = verticalWallsOriginal[transposedCoords.Y, transposedCoords.W_x];
                        verticalWalls[y, w_x] = new WallSection(wallToTranspose.IsPassable,
                            wallToTranspose.HasHamster, wallToTranspose.IsExit, wallToTranspose.IsExterior);
                    }
                }
            }
            else // Wall rotation
            {
                if (operation.GetType() == typeof(BoardRotation))
                {
                    var rotation = (BoardRotation)operation;
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var transposedCoords = transposeXYCoordinates(x, y, operation);

                            var coordsOfOriginalLeftWall = HelperMethods.GetCoordinateOfWallLeftOf(x, y);
                            var coordsOfOriginalRightWall = HelperMethods.GetCoordinateOfWallRightOf(x, y);
                            var coordsOfOriginalWallAbove = HelperMethods.GetCoordinateOfWallAbove(x, y);
                            var coordsOfOriginalWallBelow = HelperMethods.GetCoordinateOfWallBelow(x, y);

                            var originalLeftWall = verticalWallsOriginal[coordsOfOriginalLeftWall.Y, 
                                coordsOfOriginalLeftWall.W_x];
                            var originalRightWall = verticalWallsOriginal[coordsOfOriginalRightWall.Y,
                                coordsOfOriginalRightWall.W_x];
                            var originalWallBelow = horizontalWallsOriginal[coordsOfOriginalWallBelow.X,
                                coordsOfOriginalWallBelow.W_y];
                            var originalWallAbove = horizontalWallsOriginal[coordsOfOriginalWallAbove.X,
                                coordsOfOriginalWallAbove.W_y];

                            var coordsOfRotatedLeftWall = 
                                HelperMethods.GetCoordinateOfWallLeftOf(transposedCoords.X, transposedCoords.Y);
                            var coordsOfRotatedRightWall = 
                                HelperMethods.GetCoordinateOfWallRightOf(transposedCoords.X, transposedCoords.Y);
                            var coordsOfRotatedWallAbove =
                                HelperMethods.GetCoordinateOfWallAbove(transposedCoords.X, transposedCoords.Y);
                            var coordsOfRotatedWallBelow = 
                                HelperMethods.GetCoordinateOfWallBelow(transposedCoords.X, transposedCoords.Y);

                            if (rotation.HowMany90DegreesToRotateRight == 1)
                            {
                                verticalWalls[coordsOfRotatedLeftWall.Y, coordsOfRotatedLeftWall.W_x] =
                                    originalWallBelow;
                                verticalWalls[coordsOfRotatedRightWall.Y, coordsOfRotatedRightWall.W_x] =
                                    originalWallAbove;
                                horizontalWalls[coordsOfRotatedWallBelow.X, coordsOfRotatedWallBelow.W_y] =
                                    originalRightWall;
                                horizontalWalls[coordsOfRotatedWallAbove.X, coordsOfRotatedWallAbove.W_y] =
                                    originalLeftWall;
                            }
                            else if (rotation.HowMany90DegreesToRotateRight == 2)
                            {
                                verticalWalls[coordsOfRotatedLeftWall.Y, coordsOfRotatedLeftWall.W_x] =
                                    originalRightWall;
                                verticalWalls[coordsOfRotatedRightWall.Y, coordsOfRotatedRightWall.W_x] =
                                    originalLeftWall;
                                horizontalWalls[coordsOfRotatedWallBelow.X, coordsOfRotatedWallBelow.W_y] =
                                    originalWallAbove;
                                horizontalWalls[coordsOfRotatedWallAbove.X, coordsOfRotatedWallAbove.W_y] =
                                    originalWallBelow;
                            }
                            else if (rotation.HowMany90DegreesToRotateRight == 3)
                            {
                                verticalWalls[coordsOfRotatedLeftWall.Y, coordsOfRotatedLeftWall.W_x] =
                                    originalWallAbove;
                                verticalWalls[coordsOfRotatedRightWall.Y, coordsOfRotatedRightWall.W_x] =
                                    originalWallBelow;
                                horizontalWalls[coordsOfRotatedWallBelow.X, coordsOfRotatedWallBelow.W_y] =
                                    originalLeftWall;
                                horizontalWalls[coordsOfRotatedWallAbove.X, coordsOfRotatedWallAbove.W_y] =
                                    originalRightWall;
                            }
                        }
                    }
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
                new BoardFlip() { AxisToFlipAbout = PlayfieldAxis.Horizontal });
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
                int resultX, resultY;
                var rotation = (BoardRotation)transpositionToPerform;

                switch (rotation.HowMany90DegreesToRotateRight)
                {
                    case 0:
                        resultX = x;
                        resultY = y;
                        break;
                    case 1:
                        resultX = height - y - 1;
                        resultY = x;
                        break;
                    case 2:
                        resultX = width - x - 1;
                        resultY = height - y - 1;
                        break;
                    case 3:
                        resultX = y;
                        resultY = width - x - 1;
                        break;
                    default:
                        throw new LabyrinthInvalidStateException(
                            "Rotation degree must be in [0,3], was " +
                            rotation.HowMany90DegreesToRotateRight);
                }

                return new Position(resultX, resultY);

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
                int resultX, resultW_y;

                var operation = (BoardFlip)transpositionToPerform;
                if (operation.AxisToFlipAbout == PlayfieldAxis.Horizontal)
                {
                    resultX = x;
                    resultW_y = height - w_y;
                }
                else // if (operation.AxisToFlipAbout == PlayfieldAxis.Vertical)
                {
                    resultX = width - x - 1;
                    resultW_y = w_y;
                }
                return new HorizontalWallCoordinate(resultX, resultW_y);
            }
            else
            {
                throw new LabyrinthInvalidStateException(
                    "Transposing wall coordinates is only supported for flipping, not rotation.");
            }
        }

        private VerticalWallCoordinate transposeVerticalWallCoordinates(int y, int w_x,
            BoardTranspositionOperation transpositionToPerform)
        {
            if (transpositionToPerform.GetType() == typeof(BoardFlip))
            {
                int resultY, resultW_x;

                var operation = (BoardFlip)transpositionToPerform;
                if (operation.AxisToFlipAbout == PlayfieldAxis.Horizontal)
                {
                    resultY = height - y - 1;
                    resultW_x = w_x;
                }
                else // if (operation.AxisToFlipAbout == PlayfieldAxis.Vertical)
                {
                    resultY = y;
                    resultW_x = width - w_x;
                }
                return new VerticalWallCoordinate(resultY, resultW_x);
            }
            else
            {
                throw new LabyrinthInvalidStateException(
                    "Transposing wall coordinates is only supported for flipping, not rotation.");
            }
        }

        private void FlipDimensionsOfWorkingCopy()
        {
            throw new NotImplementedException();

            // TODO: Re-initialize array-based data structures and those that are dependent
            // on width and height
        }

        private bool IsRotationOf90Or270Degrees(BoardTranspositionOperation operation)
        {
            var operationIsRotation = operation.GetType() == typeof(BoardRotation);

            if (operationIsRotation)
            {
                return ((BoardRotation)operation).HowMany90DegreesToRotateRight == 1 ||
                    ((BoardRotation)operation).HowMany90DegreesToRotateRight == 3;
            }

            return false;
        }
    }
}