﻿using LabyrinthEngine.LevelConstruction;
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
            var scrambler = new BoardScrambler(originalBoard, 0, scrambleTreasureLocations: true, randomNumberGeneratorSeed: 1337);
            var boardWithScrambledTeleporterLocations = scrambler.ReturnScrambledBoard();

            Assert.True(boardWithScrambledTeleporterLocations.PlayfieldGrid[1, 1].Type != SquareType.Teleporter);
            Assert.AreEqual(boardWithScrambledTeleporterLocations.Holes.Count, originalBoard.Holes.Count);

            var firstTeleporter = boardWithScrambledTeleporterLocations.Holes[0];
            var shouldAlsoBeFirstTeleporter = firstTeleporter.NextHole.NextHole.NextHole.NextHole.NextHole;

            Assert.AreSame(firstTeleporter, shouldAlsoBeFirstTeleporter);
        }

        [Test]
        public void Scrambling_treasure_locations_works()
        {
            var scrambler = new BoardScrambler(originalBoard, 0, scrambleTreasureLocations: true, randomNumberGeneratorSeed: 1337);
            var boardWithScrambledTreasureLocations = scrambler.ReturnScrambledBoard();

            var numTreasuresOnOriginalBoard = originalBoard.PlayfieldGrid
                .Cast<PlayfieldSquare>()
                .Select(square => square.NumTreasures)
                .Aggregate((numTreasuresA, numTreasuresB) => numTreasuresA + numTreasuresB);

            var numTreasuresOnScrambledBoard = boardWithScrambledTreasureLocations.PlayfieldGrid
                .Cast<PlayfieldSquare>()
                .Select(square => square.NumTreasures)
                .Aggregate((numTreasuresA, numTreasuresB) => numTreasuresA + numTreasuresB);

            Assert.AreNotEqual(boardWithScrambledTreasureLocations.PlayfieldGrid[0, 0].NumTreasures, 1);
            Assert.AreEqual(numTreasuresOnOriginalBoard, numTreasuresOnScrambledBoard);
        }

        [Test]
        public void Scrambling_starting_positions_works()
        {
            var scrambler = new BoardScrambler(originalBoard, 0, scrambleStartingPositions: true, randomNumberGeneratorSeed: 1337);
            var boardWithScrambledStartingPositions = scrambler.ReturnScrambledBoard();

            var startingPositions = boardWithScrambledStartingPositions.StartingPositions;

            Assert.False(startingPositions[0].X == 2 && startingPositions[0].Y == 3);
            Assert.False(startingPositions[1].X == 1 && startingPositions[1].Y == 0);
        }

        [Test]
        public void Does_not_crash_when_scrambling_non_quadratic_boards()
        {
            BoardState nonQuadraticBoard = null;

            // TODO: Create plausible non-quadratic board. Should use its full width to test properly.

            var scrambler = new BoardScrambler(nonQuadraticBoard, 1, flipAboutHorizontalAxis: true, flipAboutVerticalAxis: true);
            var nonQuadraticBoardAfterRotation = scrambler.ReturnScrambledBoard();

            Assert.NotNull(nonQuadraticBoardAfterRotation);
        }
    }
}
