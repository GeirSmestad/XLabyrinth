using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Geometry;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardStateTests
    {
        WallSection[,] horizontalWalls;
        WallSection[,] verticalWalls;
        PlayfieldSquare[,] playfield;
        List<Teleporter> holes;
        Centaur centaur;
        List<Position> startingPositions;

        BoardState board;

        [SetUp]
        public void SetUp()
        {
            // Initialize an empty board. You can rebuild it with arbitrary content in each test.
            playfield = TestHelpers.InitializeEmptyPlayfield(5, 5);
            horizontalWalls = TestHelpers.InitializeEmptyHorizontalWalls(5, 5);
            verticalWalls = TestHelpers.InitializeEmptyVerticalWalls(5, 5);
            startingPositions = new List<Position> { new Position(0, 0) };
            holes = new List<Teleporter>();

            centaur = new Centaur(-1, -1, new List<CentaurStep>());
            playfield[1, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, 
                new Teleporter(0, null, 1, 3));

            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
        }

        [Test]
        public void TestGetPlayfieldSquareAt()
        {
            var square = board.GetPlayfieldSquareOf(1, 3);

            Assert.That(board.GetPlayfieldSquareOf(1, 3).Type == SquareType.Teleporter);
        }

        [Test]
        public void TestGetWallAbovePlayfieldCoordinate()
        {
            Assert.That(board.GetWallAbove(2, 0).IsExterior);
        }

        [Test]
        public void TestGetWallBelowPlayfieldCoordinate()
        {
            Assert.That(board.GetWallBelow(1, 4).IsExterior);
        }

        [Test]
        public void TestGetWallLeftOfPlayfieldCoordinate()
        {
            Assert.That(board.GetWallLeftOf(0, 3).IsExterior);
        }

        [Test]
        public void TestGetWallRightOfPlayfieldCoordinate()
        {
            Assert.That(board.GetWallRightOf(4, 3).IsExterior);
        }

        [Test]
        public void Deep_cloned_boards_are_equal()
        {
            var copy = HelperMethods.DeepClone(board);

            Assert.That(board.Equals(copy));
        }

        [Test]
        public void Boards_with_minor_changes_are_not_equal()
        {
            var copy1 = HelperMethods.DeepClone(board);
            var copy2 = HelperMethods.DeepClone(board);
            var copy3 = HelperMethods.DeepClone(board);
            var copy4 = HelperMethods.DeepClone(board);

            copy1.GetPlayfieldSquareOf(2, 2).NumTreasures++;
            copy2.GetWallLeftOf(0, 0).HasHamster = true;
            copy3.GetWallAbove(4, 3).IsExterior = true;
            copy4.centaur.MoveToNextPositionInPath();

            Assert.False(board.Equals(copy1));
            Assert.False(board.Equals(copy2));
            Assert.False(board.Equals(copy3));
            Assert.False(board.Equals(copy4));
        }
    }
}
