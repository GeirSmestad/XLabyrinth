using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using AndroidGui.Core.Helpers;
using AndroidGui.Core.Playfield;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardLoaderTests
    {
        [Test]
        public void When_loading_board_from_xml_should_yield_correct_board()
        {
            string boardXmlContent = System.IO.File.ReadAllText(@"..\..\Data\DummyBoard.xml");

            BoardState board = BoardLoader.InitializeFromXml(boardXmlContent);

            Assert.NotNull(board);

            // TODO: Add more assertions depending on DummyBoard contents
        }
    }
}
