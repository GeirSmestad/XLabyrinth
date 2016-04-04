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
            Assert.That(board.GetPlayfieldSquareAt(1, 3).Type == SquareType.Teleporter);
        }

        [Test]
        public void TestGetWallAbovePlayfieldCoordinate()
        {
            Assert.That(board.GetWallAbovePlayfieldCoordinate(2, 0).IsExterior);
        }

        [Test]
        public void TestGetWallBelowPlayfieldCoordinate()
        {
            Assert.That(board.GetWallBelowPlayfieldCoordinate(1, 4).IsExterior);
        }

        [Test]
        public void TestGetWallLeftOfPlayfieldCoordinate()
        {
            Assert.That(board.GetWallLeftOfPlayfieldCoordinate(0, 3).IsExterior);
        }

        [Test]
        public void TestGetWallRightOfPlayfieldCoordinate()
        {
            Assert.That(board.GetWallRightOfPlayfieldCoordinate(4, 3).IsExterior);
        }
    }
}
