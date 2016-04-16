using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Playfield;
using LabyrinthEngine;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardLoaderTests
    {
        BoardState board;

        [SetUp]
        public void Setup()
        {
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\BoardLoaderTestBoard.xml");

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
            Assert.That(board.GetPlayfieldSquareOf(0,0).Type == SquareType.Empty);
            Assert.That(board.GetPlayfieldSquareOf(4,4).Type == SquareType.Empty);
        }

        [Test]
        public void Fitness_studios_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareOf(2,0).Type == SquareType.FitnessStudio);
            Assert.That(board.GetPlayfieldSquareOf(3,0).Type == SquareType.FitnessStudio);
        }

        [Test]
        public void Ammo_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareOf(0,1).Type == SquareType.AmmoStorage);
        }

        [Test]
        public void Teleporters_are_parsed()
        {
            var firstTeleporter = board.GetPlayfieldSquareOf(1, 3).Hole;
            var secondTeleporter = board.GetPlayfieldSquareOf(4, 3).Hole;
            var thirdTeleporter = board.GetPlayfieldSquareOf(4, 0).Hole;
            var fourthTeleporter = board.GetPlayfieldSquareOf(3, 4).Hole;
            var fifthTeleporter = board.GetPlayfieldSquareOf(4, 1).Hole;

            Assert.That(firstTeleporter.TeleporterIndex == 1);
            Assert.That(secondTeleporter.TeleporterIndex == 2);
            Assert.That(thirdTeleporter.TeleporterIndex == 3);
            Assert.That(fourthTeleporter.TeleporterIndex == 5); // Test non-contiguous numbering
            Assert.That(fifthTeleporter.TeleporterIndex == 6);

            Assert.That(firstTeleporter.NextHole == secondTeleporter);
            Assert.That(secondTeleporter.NextHole == thirdTeleporter);
            Assert.That(thirdTeleporter.NextHole == fourthTeleporter);
            Assert.That(fourthTeleporter.NextHole == fifthTeleporter);
            Assert.That(fifthTeleporter.NextHole == firstTeleporter);
        }

        [Test]
        public void Teleporters_are_added_to_board()
        {
            Assert.That(board.Holes[0].TeleporterIndex == 1);
            Assert.That(board.Holes[1].TeleporterIndex == 2);
            Assert.That(board.Holes[2].TeleporterIndex == 3);
            Assert.That(board.Holes[3].TeleporterIndex == 5); // Test non-contiguous numbering
            Assert.That(board.Holes[4].TeleporterIndex == 6);

            Assert.That(board.Holes[0].X == 1 && board.Holes[0].Y == 3);
        }

        [Test]
        public void Playfield_coordinates_are_populated()
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    var playfieldSquare = board.GetPlayfieldSquareOf(x, y);
                    Assert.That(playfieldSquare.X == x && playfieldSquare.Y == y);
                }
            }
        }

        [Test]
        public void Hamster_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareOf(4,2).Type == SquareType.HamsterStorage);
        }

        [Test]
        public void Cement_storage_is_parsed()
        {
            Assert.That(board.GetPlayfieldSquareOf(3,2).Type == SquareType.CementStorage);
        }

        [Test]
        public void Treasures_are_parsed()
        {
            Assert.That(board.GetPlayfieldSquareOf(2, 0).NumTreasures == 2);
            Assert.That(board.GetPlayfieldSquareOf(1, 0).NumTreasures == 1);
        }

        /* Tests of wall parsing */
        [Test]
        public void Exterior_walls_are_marked_exterior()
        {
            Assert.That(board.GetWallLeftOf(0, 3).IsExterior);
            Assert.That(board.GetWallAbove(2, 0).IsExterior);
            Assert.That(board.GetWallBelow(1, 4).IsExterior);
            Assert.That(board.GetWallRightOf(4, 3).IsExterior);
        }

        [Test]
        public void Exterior_walls_are_impassable()
        {
            Assert.IsFalse(board.GetWallLeftOf(0, 3).IsPassable);
            Assert.IsFalse(board.GetWallAbove(2, 0).IsPassable);
            Assert.IsFalse(board.GetWallBelow(1, 4).IsPassable);
            Assert.IsFalse(board.GetWallRightOf(4, 3).IsPassable);
        }

        [Test]
        public void Internal_walls_are_impassable()
        {
            Assert.IsFalse(board.GetWallBelow(2, 2).IsPassable);
            Assert.IsFalse(board.GetWallRightOf(2, 4).IsPassable);
        }

        [Test]
        public void Hamstered_walls_are_marked_hamstered()
        {
            Assert.True(board.GetWallAbove(1, 3).HasHamster);
            Assert.True(board.GetWallRightOf(4,4).HasHamster);
        }

        [Test]
        public void Exits_are_parsed_correctly()
        {
            Assert.True(board.GetWallRightOf(4, 4).IsExit);
            Assert.False(board.GetWallRightOf(4, 4).IsPassable);

            Assert.True(board.GetWallBelow(4,4).IsExit);
            Assert.True(board.GetWallBelow(4, 4).IsPassable);
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
