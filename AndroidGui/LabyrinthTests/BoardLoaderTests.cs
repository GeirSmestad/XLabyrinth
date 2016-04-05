using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Playfield;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardLoaderTests
    {
        BoardState board;

        [SetUp]
        public void Setup()
        {
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\TestBoard.xml");

            var boardLoader = new BoardLoader(boardXmlContent);
            board = boardLoader.Board;
        }

        [Test]
        public void When_loading_board_from_xml_should_yield_board()
        {
            Assert.NotNull(board);
        }

        /* Tests of playfield parsing */ 

        [Test]
        public void Empty_rooms_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(0,0).Type == SquareType.Empty);
            Assert.That(board.GetPlayfieldSquareAt(4,4).Type == SquareType.Empty);
        }

        [Test]
        public void Fitness_studios_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(2,0).Type == SquareType.FitnessStudio);
            Assert.That(board.GetPlayfieldSquareAt(3,0).Type == SquareType.FitnessStudio);
        }

        [Test]
        public void Ammo_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(0,1).Type == SquareType.AmmoStorage);
        }

        [Test]
        public void Teleporters_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(1,3).Hole.TeleporterIndex == 1);
            Assert.That(board.GetPlayfieldSquareAt(4,3).Hole.TeleporterIndex == 2);
            Assert.That(board.GetPlayfieldSquareAt(4,0).Hole.TeleporterIndex == 3);
            Assert.That(board.GetPlayfieldSquareAt(3,4).Hole.TeleporterIndex == 4);
            Assert.That(board.GetPlayfieldSquareAt(4,1).Hole.TeleporterIndex == 5);
        }

        [Test]
        public void Hamster_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(4,2).Type == SquareType.HamsterStorage);
        }

        [Test]
        public void Cement_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(3,2).Type == SquareType.CementStorage);
        }

        [Test]
        public void Treasures_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareAt(2, 0).NumTreasures == 2);
            Assert.That(board.GetPlayfieldSquareAt(1, 0).NumTreasures == 1);
        }

        /* Tests of wall parsing */
        [Test]
        public void Exterior_walls_are_marked_exterior()
        {
            Assert.That(board.GetWallLeftOfPlayfieldCoordinate(0, 3).IsExterior);
            Assert.That(board.GetWallAbovePlayfieldCoordinate(2, 0).IsExterior);
            Assert.That(board.GetWallBelowPlayfieldCoordinate(1, 4).IsExterior);
            Assert.That(board.GetWallRightOfPlayfieldCoordinate(4, 3).IsExterior);
        }

        [Test]
        public void Exterior_walls_are_impassable()
        {
            Assert.IsFalse(board.GetWallLeftOfPlayfieldCoordinate(0, 3).IsPassable);
            Assert.IsFalse(board.GetWallAbovePlayfieldCoordinate(2, 0).IsPassable);
            Assert.IsFalse(board.GetWallBelowPlayfieldCoordinate(1, 4).IsPassable);
            Assert.IsFalse(board.GetWallRightOfPlayfieldCoordinate(4, 3).IsPassable);
        }

        [Test]
        public void Internal_walls_are_impassable()
        {
            Assert.IsFalse(board.GetWallBelowPlayfieldCoordinate(2, 2).IsPassable);
            Assert.IsFalse(board.GetWallRightOfPlayfieldCoordinate(2, 4).IsPassable);
        }

        [Test]
        public void Hamstered_walls_are_marked_hamstered()
        {
            Assert.True(board.GetWallAbovePlayfieldCoordinate(1, 3).HasHamster);
            Assert.True(board.GetWallRightOfPlayfieldCoordinate(4,4).HasHamster);
        }

        [Test]
        public void Exits_are_parsed_correctly()
        {
            Assert.True(board.GetWallRightOfPlayfieldCoordinate(4, 4).IsExit);
            Assert.False(board.GetWallRightOfPlayfieldCoordinate(4, 4).IsPassable);

            Assert.True(board.GetWallBelowPlayfieldCoordinate(4,4).IsExit);
            Assert.True(board.GetWallBelowPlayfieldCoordinate(4, 4).IsPassable);
        }

        [Test]
        public void Starting_positions_are_parsed()
        {
            Assert.True(board.StartingPositions[0].Equals(new Position(3, 2)));
            Assert.True(board.StartingPositions[1].Equals(new Position(4, 4)));
        }

        [Test]
        public void Centaur_is_parsed()
        {
            var centaur = board.centaur;
            Assert.True(centaur.X == -1 && centaur.Y == -1);
            Assert.True(centaur.Path[0].X == 1 && centaur.Path[0].Y == 1 && 
                centaur.Path[0].IgnoreWallsWhenSteppingHere == false);
            Assert.True(centaur.Path[1].X == 1 && centaur.Path[1].Y == 2 &&
                centaur.Path[1].IgnoreWallsWhenSteppingHere == true);
            Assert.True(centaur.Path[5].X == 1 && centaur.Path[5].Y == 2);

        }
    }
}
