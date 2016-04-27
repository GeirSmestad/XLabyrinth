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
    public class BoardSaverTests
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
        public void BoardSaver_renders_xml_output()
        {
            BoardSaver saver = new BoardSaver(board);
            var representation = saver.GenerateXmlRepresentationOfBoard();
            Assert.False(string.IsNullOrEmpty(representation));
            Assert.True(representation.Contains("LabyrinthLevel"));
        }

        [Test]
        public void Rendered_board_can_be_parsed_by_BoardLoader()
        {
            BoardSaver saver = new BoardSaver(board);
            var xmlRepresentationOfSavedBoard = saver.GenerateXmlRepresentationOfBoard();

            BoardLoader loader = new BoardLoader(xmlRepresentationOfSavedBoard);
            Assert.NotNull(loader.Board);
        }

        [Test]
        public void Rendered_board_is_equal_to_original()
        {
            BoardSaver saver = new BoardSaver(board);
            var xmlRepresentationOfSavedBoard = saver.GenerateXmlRepresentationOfBoard();

            BoardLoader loader = new BoardLoader(xmlRepresentationOfSavedBoard);
            Assert.True(board.Equals(loader.Board));
        }
    }
}
