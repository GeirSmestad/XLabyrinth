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
            string rotatedBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\RotatedRight90.xml");

            var boardLoader = new BoardLoader(rotatedBoardXmlContent);
            var correctlyRotatedBoard = boardLoader.Board;

            var scrambler = new BoardScrambler(originalBoard, 1);
            var boardRotated90DegreesRight = scrambler.ReturnScrambledBoard();

            Assert.That(boardRotated90DegreesRight.Equals(correctlyRotatedBoard));
        }

        [Test]
        public void Rotating_180_degrees_right_works()
        {
            string rotatedBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\RotatedRight180.xml");

            var boardLoader = new BoardLoader(rotatedBoardXmlContent);
            var correctlyRotatedBoard = boardLoader.Board;

            var scrambler = new BoardScrambler(originalBoard, 2);
            var boardRotated180DegreesRight = scrambler.ReturnScrambledBoard();

            Assert.That(boardRotated180DegreesRight.Equals(correctlyRotatedBoard));
        }

        [Test]
        public void Rotating_270_degrees_right_works()
        {
            string rotatedBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\RotatedRight270.xml");

            var boardLoader = new BoardLoader(rotatedBoardXmlContent);
            var correctlyRotatedBoard = boardLoader.Board;

            var scrambler = new BoardScrambler(originalBoard, 3);
            var boardRotated270DegreesRight = scrambler.ReturnScrambledBoard();

            Assert.That(boardRotated270DegreesRight.Equals(correctlyRotatedBoard));
        }

        [Test]
        public void Flipping_board_about_vertical_axis_works()
        {
            string flippedBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\FlippedAboutVertical.xml");

            var boardLoader = new BoardLoader(flippedBoardXmlContent);
            var correctlyFlippedBoard = boardLoader.Board;

            var scrambler = new BoardScrambler(originalBoard, 0, flipAboutVerticalAxis:true);
            var boardFlippedAboutVerticalAxis = scrambler.ReturnScrambledBoard();

            Assert.That(boardFlippedAboutVerticalAxis.Equals(correctlyFlippedBoard));
        }

        [Test]
        public void Flipping_board_about_horizontal_axis_works()
        {
            string flippedBoardXmlContent = System.IO.File.ReadAllText(
                @"..\..\Data\BoardScramblerTestBoards\FlippedAboutHorizontal.xml");

            var boardLoader = new BoardLoader(flippedBoardXmlContent);
            var correctlyFlippedBoard = boardLoader.Board;

            var scrambler = new BoardScrambler(originalBoard, 0, flipAboutHorizontalAxis: true);
            var boardFlippedAboutHorizontalAxis = scrambler.ReturnScrambledBoard();

            Assert.That(boardFlippedAboutHorizontalAxis.Equals(correctlyFlippedBoard));
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

        [Test]
        public void Does_not_crash_when_scrambling_non_quadratic_boards()
        {
            Assert.Fail("Not implemented");
        }
    }
}
