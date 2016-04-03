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
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\TestBoard.xml");

            BoardState board = BoardLoader.InitializeFromXml(boardXmlContent);

            Assert.NotNull(board);
        }

        [Test]
        public void When_loading_board_from_xml_should_load_all_board_properties()
        {
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\TestBoard.xml");

            BoardState board = BoardLoader.InitializeFromXml(boardXmlContent);

            Assert.Fail("Not implemented");

            // TODO: Test the returned board to test all features, using public methods.
        }
    }
}
