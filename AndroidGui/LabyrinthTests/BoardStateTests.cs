using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Helpers;

namespace LabyrinthTests
{
    [TestFixture]
    public class BoardStateTests
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
        public void TestGetPlayfieldSquareAt()
        {
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
    }
}
