using LabyrinthEngine.LevelConstruction;
using LabyrinthEngine.Playfield;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardScramblerTests
    {
        BoardState originalBoard;

        [SetUp]
        public void Setup()
        {
            string originalBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\Original.xml");

            var boardLoader = new BoardLoader(originalBoardXmlContent);
            originalBoard = boardLoader.Board;
        }

        [Test]
        public void Rotating_90_degrees_right_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Rotating_180_degrees_right_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Rotating_270_degrees_right_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Flipping_board_about_horizontal_axis_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Flipping_board_about_vertical_axis_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Scrambling_teleporters_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Scrambling_treasure_locations_works()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Scrambling_starting_positions_works()
        {
            Assert.Fail("Not implemented");
        }
    }
}
