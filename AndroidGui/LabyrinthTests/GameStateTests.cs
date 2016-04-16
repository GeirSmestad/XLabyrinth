using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using LabyrinthEngine;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Moves;
using System.Collections.ObjectModel;

namespace AndroidGui.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        WallSection[,] horizontalWalls;
        WallSection[,] verticalWalls;
        PlayfieldSquare[,] playfield;
        List<Teleporter> holes;
        Centaur centaur;
        List<Position> startingPositions;

        BoardState board;
        GameState game;

        List<Player> players;
        Player player1;

        // TODO: This test file is getting very long -- should split up in separate test files.
        // Need to think out a decent way to initialize the game state tests; they use the same functions.

        [SetUp]
        public void SetUp()
        {
            // Initialize an empty board. You can rebuild it with arbitrary content in each test.
            playfield = initializeEmptyPlayfield(5, 5);
            horizontalWalls = initializeEmptyHorizontalWalls(5, 5);
            verticalWalls = initializeEmptyVerticalWalls(5, 5);
            startingPositions = new List<Position> { new Position(0, 0) };
            holes = new List<Teleporter>();

            centaur = new Centaur(-1, -1, new List<CentaurStep>());

            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);

            player1 = new Player() { Name = "Geir" };
            players = new List<Player>();
            players.Add(player1);

            game = new GameState(board, players);
        }

        // TODO: When rewriting tests to support undo/redo, we can use properties, e.g. like
        //public Player player9
        //{
        //    get { return game.Players[9];  }
        //}

        // Game state tests

        [Test]
        public void When_starting_game_players_should_be_at_starting_positions()
        {
            // TODO: Test correct starting positions when more positions than players
            // TODO: Test correct starting positions same number of positions as players
            // TODO: Test correct starting positions when less positions than players

            Assert.Fail("Not implemented");
        }

        [Test]
        public void At_end_of_turn_centaur_should_move()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(0,0,false),
                new CentaurStep(1,0,false)
            };
            centaur = new Centaur(-1, -1, centaurMoves);

            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
        }

        [Test]
        public void Centaur_with_no_moves_should_remain_stationary()
        {
            var centaurMoves = new List<CentaurStep>();
            centaur = new Centaur(1, 1, centaurMoves);

            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
        }

        [Test]
        public void Centaur_with_only_one_move_should_remain_stationary_except_for_initial_move()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
            };
            centaur = new Centaur(0, 0, centaurMoves);

            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
        }

        [Test]
        public void Centaur_should_rotate_through_its_move_list_in_both_directions_and_reverse_correctly()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(0,0,false),
                new CentaurStep(1,0,false),
                new CentaurStep(3,3,false)
            };
            centaur = new Centaur(-1, -1, centaurMoves);

            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            // Forwards
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            // Backwards
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 3);

            // Repeat, except with the shortest path possible
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(0,0,false),
                new CentaurStep(1,0,false),
            };
            centaur = new Centaur(-1, -1, centaurMoves);

            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
        }

        [Test]
        public void Centaur_should_move_through_wall_only_when_explicitly_specified()
        {
            buildWallsAroundSquare(2, 2);
            var centaurMoves = new List<CentaurStep>()
            {
                // Move through four walls from both directions, but crash in wall
                // immediately afterwards
                new CentaurStep(1,2,true),
                new CentaurStep(2,2,true),
                new CentaurStep(2,1,true),
                new CentaurStep(2,2,true),
                new CentaurStep(3,2,true),
                new CentaurStep(2,2,true),
                new CentaurStep(2,3,true),
                new CentaurStep(2,2,true),
                new CentaurStep(2,1,false),
            };
            centaur = new Centaur(2, 2, centaurMoves);
            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 2);
            makeCurrentPlayerDoNothing();

            Assert.IsFalse(centaur.X == 2 && centaur.Y == 1);
        }

        [Test]
        public void When_blocked_by_wall_centaur_should_reverse()
        {
            // First case: Left-to-right movement
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(2,1,false),
                new CentaurStep(3,1,false),
                new CentaurStep(4,1,false),
                new CentaurStep(3,1,false),
                new CentaurStep(2,1,false)
            };
            centaur = new Centaur(0, 1, centaurMoves);
            player1.X = 0;
            player1.Y = 0;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(2, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 1);

            // Second case: Up-and-down movement
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,3,false),
                new CentaurStep(1,4,false),
                new CentaurStep(1,3,false),
                new CentaurStep(1,2,false)
            };
            centaur = new Centaur(1, 0, centaurMoves);
            player1.X = 0;
            player1.Y = 0;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 2).IsPassable = false;

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 2);
        }

        [Test]
        public void When_walled_in_centaur_should_stop_moving()
        {
            // First case: Left-to-right movement
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(2,1,false),
                new CentaurStep(1,1,false),
                new CentaurStep(0,1,false)
            };
            centaur = new Centaur(0, 1, centaurMoves);
            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);

            // Second case: Up-and-down movement
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur = new Centaur(1, 0, centaurMoves);
            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
        }

        [Test]
        public void When_released_walled_in_centaur_should_start_moving_again()
        {
            // First case: Centaur is walled in when moving, and released to the right
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(2,1,false),
                new CentaurStep(1,1,false),
                new CentaurStep(0,1,false)
            };
            centaur = new Centaur(0, 1, centaurMoves);
            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            game.Board.GetWallRightOf(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 2 && centaur.Y == 1); // Centaur resumes to the right

            // Second case: Centaur is walled in when moving, and released to the left
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(2,1,false),
                new CentaurStep(1,1,false),
                new CentaurStep(0,1,false)
            };
            centaur = new Centaur(0, 1, centaurMoves);
            player1.X = 4;
            player1.Y = 4;
            game.Board.GetWallLeftOf(1, 1).IsPassable = true; // Reset walls for this
            game.Board.GetWallRightOf(1, 1).IsPassable = true;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            game.Board.GetWallLeftOf(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 1); // Centaur resumes to the left
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);

            // Third case: Centaur is walled in when moving, and released upwards
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur = new Centaur(1, 0, centaurMoves);
            player1.X = 4;
            player1.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            game.Board.GetWallAbove(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0); // Centaur resumes up

            // Fourth case: Centaur is walled in when moving, and released downwards
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur = new Centaur(1, 0, centaurMoves);
            player1.X = 4;
            player1.Y = 4;
            game.Board.GetWallAbove(1, 1).IsPassable = true; // Reset walls for this
            game.Board.GetWallBelow(1, 1).IsPassable = true;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);
            game.Board.GetWallBelow(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 2); // Centaur resumes down
        }

        [Test]
        public void When_moving_to_centaur_player_should_die()
        {
            var centaurMoves = new List<CentaurStep>();
            centaur = new Centaur(1, 1, centaurMoves);

            player1.X = 2;
            player1.Y = 1;
            player1.IsAlive = true;

            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveLeft);

            Assert.IsFalse(game.Players[0].IsAlive);
        }

        [Test]
        public void When_hit_by_centaur_player_should_die()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(2,2, false),
                new CentaurStep(2,3, false)
            };
            centaur = new Centaur(-1, -1, centaurMoves);

            player1.X = 2;
            player1.Y = 2;
            player1.IsAlive = true;

            var player2 = new Player() { Name = "Nemesis", IsAlive = true};
            players.Add(player2);
            player2.X = 2;
            player2.Y = 3;
            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsFalse(game.Players[0].IsAlive);
            Assert.IsTrue(game.Players[1].IsAlive);
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsFalse(game.Players[1].IsAlive);
        }

        [Test]
        public void When_player_moves_near_centaur_should_print_clopclop()
        {
            var centaurMoves = new List<CentaurStep>();
            centaur = new Centaur(2, 2, centaurMoves);

            player1.X = 0;
            player1.Y = 2;

            var player2 = new Player() { Name = "player2", X = 2, Y = 0 };
            var player3 = new Player() { Name = "player2", X = 2, Y = 4 };
            var player4 = new Player() { Name = "player2", X = 4, Y = 2 };

            players.Add(player2);
            players.Add(player3);
            players.Add(player4);

            initializeNewGameStateFromSetupParameters();

            var message = game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(message.Contains("clop"));

            message = game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(message.Contains("clop"));

            message = game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(message.Contains("clop"));

            message = game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(message.Contains("clop"));
        }

        [Test]
        public void When_centaur_moves_near_player_should_print_clopclop()
        {
            // Move in square pattern to approach each player from a separate direction
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(2,1, false),
                new CentaurStep(3,1, false),
                new CentaurStep(3,2, false),
                new CentaurStep(2,2, false),
            };
            centaur = new Centaur(1, 1, centaurMoves);

            player1.X = 2;
            player1.Y = 0;

            var player2 = new Player() { Name = "player2", X = 4, Y = 1 };
            var player3 = new Player() { Name = "player3", X = 3, Y = 3 };
            var player4 = new Player() { Name = "player4", X = 1, Y = 2 };

            players.Add(player2);
            players.Add(player3);
            players.Add(player4);

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(game.Players[0].Name));

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(game.Players[1].Name));

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(game.Players[2].Name));

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(game.Players[3].Name));
        }

        [Test]
        public void When_moving_to_hole_player_should_move_to_next_hole()
        {
            player1.X = 3;
            player1.Y = 2;

            var teleporter3 = new Teleporter(2, null, 4, 3);
            var teleporter2 = new Teleporter(1, teleporter3, 4, 4);
            var teleporter1 = new Teleporter(0, teleporter2, 3, 3);
            teleporter3.NextHole = teleporter1;
            holes.Add(teleporter1);
            holes.Add(teleporter2);
            holes.Add(teleporter3);
            playfield[3, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter1);
            playfield[4, 4] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter2);
            playfield[4, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter3);
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveDown);
            Assert.That(player1.X == 4 && player1.Y == 4);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(player1.X == 4 && player1.Y == 4);

            game.PerformMove(MoveType.MoveUp);
            Assert.That(player1.X == 3 && player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(player1.X == 3 && player1.Y == 3);
        }

        [Test]
        public void When_jumping_into_hole_player_should_move_to_next_hole_but_not_when_waiting()
        {
            player1.X = 3;
            player1.Y = 3;

            var teleporter3 = new Teleporter(2, null, 4, 3);
            var teleporter2 = new Teleporter(1, teleporter3, 4, 4);
            var teleporter1 = new Teleporter(0, teleporter2, 3, 3);
            teleporter3.NextHole = teleporter1;
            holes.Add(teleporter1);
            holes.Add(teleporter2);
            holes.Add(teleporter3);
            playfield[3, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter1);
            playfield[4, 4] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter2);
            playfield[4, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter3);
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            Assert.That(player1.X == 3 && player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(player1.X == 3 && player1.Y == 3);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(player1.X == 4 && player1.Y == 4);
            game.PerformMove(MoveType.FallThroughHole); // Invalid operation, should have no effect
            Assert.That(player1.X == 4 && player1.Y == 4);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(player1.X == 4 && player1.Y == 3);
            game.PerformMove(MoveType.ThrowGrenadeDown);
            Assert.That(player1.X == 4 && player1.Y == 3);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(player1.X == 3 && player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(player1.X == 3 && player1.Y == 3);
        }

        [Test]
        public void When_starting_game_players_are_alive()
        {
            var player2 = new Player() { Name = "Nemesis" };
            players.Add(player2);
            initializeNewGameStateFromSetupParameters();

            Assert.IsTrue(game.Players[0].IsAlive);
            Assert.IsTrue(game.Players[1].IsAlive);
        }

        [Test]
        public void When_shot_player_should_die()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 5;

            var nemesis = new Player() { Name = "Nemesis", IsAlive = true };
            players.Add(nemesis);
            nemesis.X = 2;
            nemesis.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(nemesis.IsAlive);

            nemesis.X = 4;
            nemesis.Y = 2;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(nemesis.IsAlive);

            nemesis.X = 2;
            nemesis.Y = 4;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(nemesis.IsAlive);

            nemesis.X = 0;
            nemesis.Y = 2;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(nemesis.IsAlive);

            nemesis.X = player1.X;
            nemesis.Y = player1.Y;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsFalse(nemesis.IsAlive);

            Assert.AreEqual(player1.NumArrows, 0, "Should spend arrows when shooting.");
        }

        [Test]
        public void When_shot_through_wall_player_should_survive()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 4;
            buildWallsAroundSquare(0, 2);
            buildWallsAroundSquare(4, 2);
            buildWallsAroundSquare(2, 4);
            buildWallsAroundSquare(0, 2);

            var nemesis = new Player() { Name = "Nemesis", IsAlive = true };
            players.Add(nemesis);
            nemesis.X = 2;
            nemesis.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsTrue(nemesis.IsAlive);

            nemesis.X = 4;
            nemesis.Y = 2;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsTrue(nemesis.IsAlive);

            nemesis.X = 2;
            nemesis.Y = 4;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsTrue(nemesis.IsAlive);

            nemesis.X = 0;
            nemesis.Y = 2;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsTrue(nemesis.IsAlive);
        }

        [Test]
        public void When_shooting_arrow_should_hit_the_first_player_it_encounters()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 4;

            var nemesis = new Player() { Name = "Nemesis", IsAlive = true };
            var innocentBystander = new Player() { Name = "InnocentBystander", IsAlive = true };
            players.Add(nemesis);
            players.Add(innocentBystander);

            nemesis.X = 2;
            nemesis.Y = 1;
            innocentBystander.X = 2;
            innocentBystander.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(nemesis.IsAlive);
            Assert.IsTrue(innocentBystander.IsAlive);

            nemesis.X = 3;
            nemesis.Y = 2;
            innocentBystander.X = 4;
            innocentBystander.Y = 2;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(nemesis.IsAlive);
            Assert.IsTrue(innocentBystander.IsAlive);

            nemesis.X = 2;
            nemesis.Y = 3;
            innocentBystander.X = 2;
            innocentBystander.Y = 4;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(nemesis.IsAlive);
            Assert.IsTrue(innocentBystander.IsAlive);

            nemesis.X = 1;
            nemesis.Y = 2;
            innocentBystander.X = 0;
            innocentBystander.Y = 2;
            nemesis.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(nemesis.IsAlive);
            Assert.IsTrue(innocentBystander.IsAlive);
        }

        [Test]
        public void Arrows_should_pass_through_dead_players()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 4;

            var deadPlayer = new Player() { Name = "DeadPlayer", IsAlive = false };
            var victim = new Player() { Name = "Victim", IsAlive = true };
            players.Add(deadPlayer);
            players.Add(victim);

            deadPlayer.X = 2;
            deadPlayer.Y = 1;
            victim.X = 2;
            victim.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(victim.IsAlive);

            deadPlayer.X = 3;
            deadPlayer.Y = 2;
            victim.X = 4;
            victim.Y = 2;
            victim.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(victim.IsAlive);

            deadPlayer.X = 2;
            deadPlayer.Y = 3;
            victim.X = 2;
            victim.Y = 4;
            victim.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(victim.IsAlive);

            deadPlayer.X = 1;
            deadPlayer.Y = 2;
            victim.X = 0;
            victim.Y = 2;
            victim.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(victim.IsAlive);
        }

        [Test]
        public void When_shooting_at_exit_arrow_should_hit_nothing()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 4;

            board.GetWallAbove(2, 0).IsExit = true;
            board.GetWallAbove(2, 0).IsPassable = true;
            board.GetWallAbove(2, 0).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.FireUp);
            Assert.IsTrue(message.Contains("missed"));

            board.GetWallRightOf(4, 2).IsExit = true;
            board.GetWallRightOf(4, 2).IsPassable = true;
            board.GetWallRightOf(4, 2).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireRight);
            Assert.IsTrue(message.Contains("missed"));

            board.GetWallBelow(2, 4).IsExit = true;
            board.GetWallBelow(2, 4).IsPassable = true;
            board.GetWallBelow(2, 4).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireDown);
            Assert.IsTrue(message.Contains("missed"));

            board.GetWallLeftOf(0, 2).IsExit = true;
            board.GetWallLeftOf(0, 2).IsPassable = true;
            board.GetWallLeftOf(0, 2).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireLeft);
            Assert.IsTrue(message.Contains("missed"));

            Assert.AreEqual(player1.NumArrows, 0);
        }

        [Test]
        public void When_shooting_centaur_should_see_message_and_miss_player_behind_centaur()
        {
            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 4;

            var innocentBystander = new Player() { Name = "InnocentBystander", IsAlive = true };
            players.Add(innocentBystander);

            centaur = new Centaur(2, 1, new List<CentaurStep>());
            innocentBystander.X = 2;
            innocentBystander.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.FireUp);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur = new Centaur(3, 2, new List<CentaurStep>());
            innocentBystander.X = 4;
            innocentBystander.Y = 2;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireRight);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur = new Centaur(2, 3, new List<CentaurStep>());
            innocentBystander.X = 2;
            innocentBystander.Y = 4;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireDown);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur = new Centaur(1, 2, new List<CentaurStep>());
            innocentBystander.X = 0;
            innocentBystander.Y = 2;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireLeft);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);
        }

        /// <summary>
        /// Ensure that when firing on a square with many players, the player that is selected
        /// randomly to die is the same when the game state is identical. This test is sensitive
        /// to changes in the RNG logic, since the player must match a particular seed.
        /// </summary>
        [Test]
        public void When_shooting_at_multiple_players_random_death_should_be_consistent()
        {
            int initialRngSeed = 1338;

            player1.X = 2;
            player1.Y = 2;
            player1.NumArrows = 5;

            var player2 = new Player() { Name = "Shrodinger1", IsAlive = true };
            var player3 = new Player() { Name = "Shrodinger2", IsAlive = true };
            var player4 = new Player() { Name = "Shrodinger3", IsAlive = true };
            var player5 = new Player() { Name = "Shrodinger4", IsAlive = true };

            // This turns out to be the unlucky victim for this particular RNG seed.
            var playerThatWillDieForThisRngSeed = player4;

            players.Add(player2);
            players.Add(player3);
            players.Add(player4);
            players.Add(player5);

            player2.X = player3.X = player4.X = player5.X = 2;
            player2.Y = player3.Y = player4.Y = player5.Y = 0;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(playerThatWillDieForThisRngSeed.IsAlive);

            player2.X = player3.X = player4.X = player5.X = 4;
            player2.Y = player3.Y = player4.Y = player5.Y = 2;
            player2.IsAlive = player3.IsAlive = player4.IsAlive = player5.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(playerThatWillDieForThisRngSeed.IsAlive);

            player2.X = player3.X = player4.X = player5.X = 2;
            player2.Y = player3.Y = player4.Y = player5.Y = 4;
            player2.IsAlive = player3.IsAlive = player4.IsAlive = player5.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(playerThatWillDieForThisRngSeed.IsAlive);

            player2.X = player3.X = player4.X = player5.X = 0;
            player2.Y = player3.Y = player4.Y = player5.Y = 2;
            player2.IsAlive = player3.IsAlive = player4.IsAlive = player5.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(playerThatWillDieForThisRngSeed.IsAlive);

            player2.X = player3.X = player4.X = player5.X = player1.X;
            player2.Y = player3.Y = player4.Y = player5.Y = player1.Y;
            player2.IsAlive = player3.IsAlive = player4.IsAlive = player5.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsFalse(playerThatWillDieForThisRngSeed.IsAlive);
        }

        [Test]
        public void When_player_uses_nonexistent_gear_nothing_should_happen()
        {
            // Test using all five different gear types in legal situation but carrying zero of each.
            // Then perform a legal move afterwards.
            // Assert that no actions except the legal move were executed.

            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_ammo_storage_player_should_replenish_weapons()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = true;
            player1.NumArrows = 0;
            player1.NumGrenades = 0;

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(player1.NumArrows, Player.ArrowCapacity);
            Assert.AreEqual(player1.NumGrenades, Player.GrenadeCapacity);

        }

        [Test]
        public void When_visiting_hamster_storage_player_should_replenish_hamster_gear()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.HamsterStorage, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = true;
            player1.NumHamsters = 0;
            player1.NumHamsterSprays = 0;

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(player1.NumHamsters, Player.HamsterCapacity);
            Assert.AreEqual(player1.NumHamsterSprays, Player.HamsterSprayCapacity);
        }

        [Test]
        public void When_visiting_fitness_studio_dead_player_should_see_it_and_be_resurrected()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.FitnessStudio, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = false;

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsTrue(player1.IsAlive);
            Assert.IsTrue(message.Contains("fitness studio"));
        }

        [Test]
        public void When_visiting_fitness_studio_live_player_should_see_empty_room()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.FitnessStudio, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = true;

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsFalse(message.Contains("fitness studio"));
        }

        [Test]
        public void Dead_player_should_not_replenish_consumables_when_visiting_storage()
        {
            playfield[2, 1] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            playfield[3, 1] = new PlayfieldSquare(SquareType.CementStorage, 0);
            playfield[4, 1] = new PlayfieldSquare(SquareType.HamsterStorage, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 1;
            player1.Y = 1;
            player1.NumArrows = 0;
            player1.NumGrenades = 0;
            player1.NumHamsters = 0;
            player1.NumHamsterSprays = 0;
            player1.NumCement = 0;
            player1.IsAlive = false;

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.MoveRight);

            Assert.AreEqual(player1.NumArrows, 0);
            Assert.AreEqual(player1.NumGrenades, 0);
            Assert.AreEqual(player1.NumHamsters, 0);
            Assert.AreEqual(player1.NumHamsterSprays, 0);
            Assert.AreEqual(player1.NumCement, 0);
        }

        [Test]
        public void Player_should_drop_treasure_and_consumables_when_killed()
        {
            player1.X = 0;
            player1.Y = 0;
            player1.NumArrows = 1;
            board.GetPlayfieldSquareOf(1, 0).NumTreasures = 0;

            var victim = new Player()
            {
                Name = "Victim",
                IsAlive = true,
                CarriesTreasure = true,
                NumArrows = 2,
                NumGrenades = 2,
                NumHamsters = 2,
                NumHamsterSprays = 2,
                NumCement = 2
            };
            players.Add(victim);
            victim.X = 1;
            victim.Y = 0;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);

            Assert.IsFalse(victim.CarriesTreasure);
            Assert.AreEqual(victim.NumArrows, 0);
            Assert.AreEqual(victim.NumGrenades, 0);
            Assert.AreEqual(victim.NumHamsters, 0);
            Assert.AreEqual(victim.NumHamsterSprays, 0);
            Assert.AreEqual(victim.NumCement, 0);
            Assert.AreEqual(board.GetPlayfieldSquareOf(victim).NumTreasures, 1);
        }

        [Test]
        public void When_visiting_cement_storage_player_should_replenish_cement()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.CementStorage, 0);
            initializeNewGameStateFromSetupParameters();

            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = true;
            player1.NumCement = 0;

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(player1.NumCement, Player.CementCapacity);
        }

        [Test]
        public void When_hitting_wall_player_should_not_move()
        {
            buildWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 1);
            Assert.AreEqual(player1.Y, 1);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 1);
            Assert.AreEqual(player1.Y, 1);

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 1);
            Assert.AreEqual(player1.Y, 1);

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 1);
            Assert.AreEqual(player1.Y, 1);
        }

        [Test]
        public void When_moving_in_open_area_player_should_move()
        {
            player1.X = 3;
            player1.Y = 3;

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 3);
            Assert.AreEqual(player1.Y, 2);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 4);
            Assert.AreEqual(player1.Y, 2);

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 4);
            Assert.AreEqual(player1.Y, 3);

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(player1.X, 3);
            Assert.AreEqual(player1.Y, 3);
        }

        [Test]
        public void When_placing_hamster_hamster_should_be_placed()
        {
            buildWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumHamsters = 4;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterUp);

            Assert.IsTrue(board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterRight);

            Assert.IsTrue(board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterDown);

            Assert.IsTrue(board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterLeft);

            Assert.IsTrue(board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(player1.NumHamsters, 0, "When placing hamster, hamster count should decrease");
        }

        [Test]
        public void When_placing_hamster_on_hamstered_wall_should_yield_no_result()
        {
            buildHamsteredWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumHamsters = 4;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterUp);

            Assert.IsTrue(board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterRight);

            Assert.IsTrue(board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterDown);

            Assert.IsTrue(board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterLeft);

            Assert.IsTrue(board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(player1.NumHamsters, 4);
        }

        [Test]
        public void When_spraying_hamsters_hamsters_should_die()
        {
            buildHamsteredWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumHamsterSprays = 4;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayUp);

            Assert.IsFalse(board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayRight);

            Assert.IsFalse(board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayDown);

            Assert.IsFalse(board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayLeft);

            Assert.IsFalse(board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(player1.NumHamsterSprays, 0, "When spraying hamsters, should spend hamster spray");
        }

        [Test]
        public void When_constructing_wall_wall_should_be_created()
        {
            player1.X = 1;
            player1.Y = 1;
            player1.NumCement = 4;
            board.GetWallAbove(1, 1).IsExterior = true;
            board.GetWallAbove(1, 1).IsExit = true;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallUp);

            Assert.IsFalse(board.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallRight);

            Assert.IsFalse(board.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallDown);

            Assert.IsFalse(board.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallLeft);

            Assert.IsFalse(board.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(player1.NumCement, 0, "When using cement, cement count should decrease");
        }

        [Test]
        public void When_attempting_to_construct_wall_on_wall_should_yield_no_result()
        {
            buildWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumCement = 4;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallUp);

            Assert.IsFalse(board.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallRight);

            Assert.IsFalse(board.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallDown);

            Assert.IsFalse(board.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallLeft);

            Assert.IsFalse(board.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(player1.NumCement, 4, "When building on existing walls, cement should not decrease");
        }

        [Test]
        public void When_blowing_up_wall_wall_should_disappear()
        {
            buildWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumGrenades = 4;
            var expectedMessage = "You blow up the wall. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(player1.NumGrenades, 0, "When using grenades, grenade count should decrease");
        }

        [Test]
        public void When_blowing_up_exterior_wall_should_see_message_and_no_result()
        {
            player1.X = 0;
            player1.Y = 0;
            player1.NumGrenades = 4;
            var expectedMessage = "The grenade explodes against an exterior wall. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallAbove(player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallLeftOf(player1).IsPassable);

            player1.X = 4;
            player1.Y = 4;

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallRightOf(player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallBelow(player1).IsPassable);

            Assert.AreEqual(player1.NumGrenades, 0);
        }

        [Test]
        public void When_blowing_up_hidden_exit_should_remove_wall()
        {
            player1.X = 0;
            player1.Y = 0;
            player1.NumGrenades = 4;
            var expectedMessage = "You blow up the wall. ";

            var firstHiddenExit = board.GetWallAbove(0, 0);
            var secondHiddenExit = board.GetWallLeftOf(0, 0);
            var thirdHiddenExit = board.GetWallRightOf(4, 4);
            var fourthHiddenExit = board.GetWallBelow(4, 4);

            firstHiddenExit.IsExit = secondHiddenExit.IsExit =
                thirdHiddenExit.IsExit = fourthHiddenExit.IsExit = true;
            firstHiddenExit.IsPassable = secondHiddenExit.IsPassable =
                thirdHiddenExit.IsPassable = fourthHiddenExit.IsPassable = false;
            firstHiddenExit.IsExterior = secondHiddenExit.IsExterior =
                thirdHiddenExit.IsExterior = fourthHiddenExit.IsExterior = true;

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(game.Board.GetWallAbove(player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(game.Board.GetWallLeftOf(player1).IsPassable);

            player1.X = 4;
            player1.Y = 4;

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(game.Board.GetWallRightOf(player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(game.Board.GetWallBelow(player1).IsPassable);
        }

        [Test]
        public void When_blowing_up_hamster_wall_should_see_message_and_no_result()
        {
            buildHamsteredWallsAroundSquare(3, 1);
            player1.X = 3;
            player1.Y = 1;
            player1.NumGrenades = 4;
            var expectedMessage = "A hamster returns your grenade with the pin inserted. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallAbove(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallRightOf(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallBelow(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallLeftOf(3, 1).IsPassable);

            Assert.AreEqual(player1.NumGrenades, 4, "Hamster walls should not spend grenades");
        }

        [Test]
        public void When_visiting_treasure_player_should_take_it()
        {
            player1.X = 3;
            player1.Y = 3;
            board.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.True(player1.CarriesTreasure);
            Assert.AreEqual(board.GetPlayfieldSquareOf(3, 2).NumTreasures, 1);
        }

        [Test]
        public void When_visiting_treasure_when_already_carrying_player_should_see_and_leave_it()
        {
            player1.X = 3;
            player1.Y = 3;
            player1.CarriesTreasure = true;
            board.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;

            string moveDescription = game.PerformMove(MoveType.MoveUp);

            Assert.True(moveDescription.Contains("There is treasure here"));
            Assert.True(player1.CarriesTreasure);
            Assert.AreEqual(board.GetPlayfieldSquareOf(3, 2).NumTreasures, 2);
        }

        [Test]
        public void When_visiting_treasure_dead_player_should_see_and_leave_it()
        {
            player1.X = 3;
            player1.Y = 3;
            player1.CarriesTreasure = false;
            player1.IsAlive = false;
            board.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;

            string moveDescription = game.PerformMove(MoveType.MoveUp);

            Assert.True(moveDescription.Contains("There is treasure here"));
            Assert.False(player1.CarriesTreasure);
            Assert.AreEqual(board.GetPlayfieldSquareOf(3, 2).NumTreasures, 2);
        }

        [Test]
        public void When_exiting_labyrinth_in_single_player_should_stay_outside_for_one_move_sequence()
        {
            player1.X = 0;
            player1.Y = 3;
            player1.NumHamsters = 1;
            player1.NumArrows = 1;
            player1.NumCement = 1;
            var exit = board.GetWallLeftOf(0, 3);

            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;

            var message1 = game.PerformMove(MoveType.MoveLeft);
            Assert.IsTrue(player1.IsOutsideLabyrinth());
            var message2 = game.PerformMove(MoveType.PlaceHamsterRight); // No effect, outside
            Assert.AreEqual(player1.NumHamsters, 1);
            Assert.IsTrue(player1.IsOutsideLabyrinth());
            var message3 = game.PerformMove(MoveType.MoveRight);  // No effect, outside
            Assert.IsTrue(player1.IsOutsideLabyrinth());
            Assert.IsTrue(player1.X == -1 && player1.Y == 3);
            var message4 = game.PerformMove(MoveType.BuildWallRight); // No effect, outside
            Assert.AreEqual(player1.NumCement, 1);

            Assert.IsFalse(player1.IsOutsideLabyrinth());
            game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(player1.X == 1 && player1.Y == 3);

            // TODO: I don't know if the original rules specify that return to inside
            // happens on the end of the second or the beginning of the third turn.
            // May have to rewrite this functionality.

            Assert.IsTrue(message1.Contains("outside"));
            Assert.IsTrue(message2.Contains("outside"));
            Assert.IsTrue(message3.Contains("outside"));
            Assert.IsTrue(message4.Contains("enter"));
        }

        [Test]
        public void When_exiting_labyrinth_player_should_return_at_end_of_second_turn()
        {
            player1.X = 0;
            player1.Y = 1;
            var exit = board.GetWallLeftOf(0, 1);

            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(player1.IsOutsideLabyrinth());
            game.PerformMove(MoveType.DoNothing);
            Assert.IsFalse(player1.IsOutsideLabyrinth());

            // TODO: I don't know if the original rules specify that return to inside
            // happens on the end of the second or the beginning of the third turn.
            // May have to rewrite this functionality.
        }

        [Test]
        public void When_exiting_labyrinth_player_should_skip_turn_and_reenter()
        {
            var player2 = new Player() { Name = "Spelunker", X = 0, Y = 0 };
            players.Add(player2);
            player1.X = 0;
            player1.Y = 0;
            player2.NumArrows = 1;

            var exit = game.Board.GetWallAbove(0, 0);

            exit.IsExterior = true;
            exit.IsPassable = true;
            exit.IsExit = true;

            game.PerformMove(MoveType.MoveUp); // Player 1 exits
            Assert.IsTrue(player1.IsOutsideLabyrinth());
            Assert.IsTrue(player1.X == 0 && player1.Y == -1);
            var player1Message = game.PerformMove(MoveType.PlaceHamsterLeft);
            Assert.IsTrue(player1Message.Contains("outside"));

            var player2Message = game.PerformMove(MoveType.MoveUp); // Player 2 exits
            Assert.IsTrue(player2.IsOutsideLabyrinth());
            Assert.IsTrue(player2.X == 0 && player2.Y == -1);
            Assert.IsTrue(player2Message.Contains("outside"));
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsTrue(player1.IsAlive);
            Assert.AreEqual(player2.NumArrows, 1);

            player1Message = game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(player1Message.Contains("outside"));
            Assert.IsTrue(player1.X == 0 && player1.Y == -1);
            player1Message = game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(player1Message.Contains("enter"));
            Assert.IsTrue(player1.X == 0 && player1.Y == 0);

            player2Message = game.PerformMove(MoveType.FireDown);
            Assert.AreEqual(player2.NumArrows, 1);
            Assert.IsFalse(player2.IsOutsideLabyrinth());
            Assert.IsTrue(player2.X == 0 && player2.Y == 0);
            Assert.IsTrue(player2Message.Contains("enter"));

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(player1.X == 0 && player1.Y == 1);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(player2.X == 1 && player2.Y == 0);
        }

        [Test]
        public void When_exiting_labyrinth_with_treasure_player_should_get_point()
        {
            player1.X = 4;
            player1.Y = 2;
            player1.CarriesTreasure = true;
            player1.Score = 0;

            var exit = board.GetWallRightOf(4, 2);

            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;

            game.PerformMove(MoveType.MoveRight);

            Assert.IsFalse(player1.CarriesTreasure);
            Assert.IsTrue(player1.Score == 1);
        }

        [Test]
        public void When_exiting_labyrinth_without_treasure_player_should_get_message_but_no_points()
        {
            player1.X = 2;
            player1.Y = 4;
            player1.CarriesTreasure = false;
            player1.Score = 0;

            var exit = board.GetWallBelow(2, 4);

            exit.IsExterior = true;
            exit.IsExit = true;
            exit.IsPassable = true;


            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsTrue(player1.Score == 0);
            Assert.IsTrue(message.Contains("outside"));
        }

        [Test]
        public void Players_should_take_turns_to_move()
        {
            Assert.Fail("Not implemented");
            // Test both movement moves and followup actions.
        }

        [Test]
        public void Movement_as_followup_action_should_be_ignored()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Followup_action_as_movement_should_skip_movement_and_execute()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        /// <summary>
        /// If as a followup action e.g. constructing a wall where there is a wall, the
        /// turn should pass to the next player to avoid exploiting this for quick exploration.
        /// </summary>
        public void Blocked_followup_actions_should_execute()
        {
            Assert.Fail("Not implemented");
        }

        // Game management tests

        [Test]
        public void Saving_and_loading_game_state_should_work()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Undo_and_redo_moves_should_work()
        {
            // Test that undo works
            // Test that redo works
            // Check that both in combination work
            // Check that undo and the resuming play does not corrupt game state

            Assert.Fail("Not implemented");
        }

        [Test]
        public void Redo_state_is_removed_after_move()
        {
            Assert.Fail("Not implemented");
        }

        // Helper methods for creating data to test.

        private void buildWallsAroundSquare(int x, int y)
        {
            //horizontalWalls[x, y] = new WallSection(false, false, false, false);
            //horizontalWalls[x, y+1] = new WallSection(false, false, false, false);
            //verticalWalls[y, x] = new WallSection(false, false, false, false);
            //verticalWalls[y, x + 1] = new WallSection(false, false, false, false);

            board.GetWallAbove(x, y).IsPassable = false;
            board.GetWallRightOf(x, y).IsPassable = false;
            board.GetWallBelow(x, y).IsPassable = false;
            board.GetWallLeftOf(x, y).IsPassable = false;
        }

        private void buildHamsteredWallsAroundSquare(int x, int y)
        {
            board.GetWallAbove(x, y).IsPassable = false;
            board.GetWallRightOf(x, y).IsPassable = false;
            board.GetWallBelow(x, y).IsPassable = false;
            board.GetWallLeftOf(x, y).IsPassable = false;

            board.GetWallAbove(x, y).HasHamster = true;
            board.GetWallRightOf(x, y).HasHamster = true;
            board.GetWallBelow(x, y).HasHamster = true;
            board.GetWallLeftOf(x, y).HasHamster = true;
        }

        private void makeCurrentPlayerDoNothing()
        {
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
        }

        private PlayfieldSquare[,] initializeEmptyPlayfield(int boardWidth, int boardHeight)
        {
            var result = new PlayfieldSquare[boardWidth, boardHeight];

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    result[x, y] = new PlayfieldSquare(SquareType.Empty, 0);
                }
            }
            return result;
        }

        private WallSection[,] initializeEmptyHorizontalWalls(int boardWidth, int boardHeight)
        {
            var result = new WallSection[boardWidth, boardHeight + 1];

            for (int w_y = 0; w_y <= boardHeight; w_y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    if (w_y == 0 || w_y == boardHeight)
                    {
                        result[x, w_y] = new WallSection(false, false, false, isExterior: true);
                    }
                    else
                    {
                        result[x, w_y] = new WallSection(true, false, false, false); // No wall
                    }
                }
            }
            return result;
        }

        private WallSection[,] initializeEmptyVerticalWalls(int boardWidth, int boardHeight)
        {
            var result = new WallSection[boardWidth, boardHeight + 1];

            for (int w_x = 0; w_x <= boardWidth; w_x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (w_x == 0 || w_x == boardWidth)
                    {
                        result[y, w_x] = new WallSection(false, false, false, isExterior: true);
                    }
                    else
                    {
                        result[y, w_x] = new WallSection(true, false, false, false); // No wall
                    }
                }
            }
            return result;
        }

        private void initializeNewGameStateFromSetupParameters()
        {
            var playerStatePreInitialization = HelperMethods.DeepClone(players);

            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            game = new GameState(board, players);

            for (int i = 0; i < game.Players.Count; i++)
            {
                // Preserve the player positions that were defined in the test, rather than
                // using the positions that the game chooses based on the board-defined
                // initial starting positions
                game.Players[i].X = playerStatePreInitialization[i].X;
                game.Players[i].Y = playerStatePreInitialization[i].Y;
            }
        }

        private void initializeNewGameStateFromSetupParameters(int withInitialRngSeed)
        {
            var playerStatePreInitialization = HelperMethods.DeepClone(players);

            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            game = new GameState(board, players, withInitialRngSeed);

            for (int i = 0; i < game.Players.Count; i++)
            {
                // Preserve the player positions that were defined in the test, rather than
                // using the positions that the game chooses based on the board-defined
                // initial starting positions
                game.Players[i].X = playerStatePreInitialization[i].X;
                game.Players[i].Y = playerStatePreInitialization[i].Y;
            }
        }
    }
}