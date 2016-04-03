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
        [Test]
        public void When_loading_board_from_xml_should_yield_board()
        {
            // TODO: Can load this in setup, no need to do in each test.
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\TestBoard.xml");

            var boardLoader = new BoardLoader(boardXmlContent);
            BoardState board = boardLoader.Board;

            Assert.NotNull(board);
        }

        [Test]
        public void TestPlayfieldParser()
        {
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\TestBoard.xml");

            var boardLoader = new BoardLoader(boardXmlContent);
            BoardState board = boardLoader.Board;

            // TODO: Test dimensions and all board properties.
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestHorizontalWallParser()
        {
            // Test exterior walls marked
            // Test interior walls marked
            // Test that expected walls appear

            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestVerticalWallParser()
        {
            // Test exterior walls marked
            // Test interior walls marked
            // Test that expected walls appear

            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestStartingPositionParser()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void TestCentaurParser()
        {
            Assert.Fail("Not implemented");
        }
    }
}
