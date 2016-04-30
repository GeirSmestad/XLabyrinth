using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using LabyrinthEngine;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Geometry;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Moves;
using System.Collections.ObjectModel;
using LabyrinthTests;
using LabyrinthEngine.GameLogic;

namespace AndroidGui.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        WallSection[,] horizontalWalls_initial;
        WallSection[,] verticalWalls_initial;
        PlayfieldSquare[,] playfield_initial;
        List<Teleporter> holes_initial;
        Centaur centaur_initial;
        List<Position> startingPositions_initial;

        BoardState board_initial;
        GameState game;

        List<Player> players_initial;
        Player player1_initial;

        // TODO: This test file is getting very long -- should split up in separate test files.
        // Need to think out a decent way to initialize the game state tests; they use the same functions.

        [SetUp]
        public void SetUp()
        {
            // Initialize an empty board. You can rebuild it with arbitrary content in each test.

            // _initial denotes that this is a reference to an object defined at the start of each test.
            // GameState creates new instances of all its objects, making the *_initial references useless
            // once the GameState object has been created.
            playfield_initial = TestHelpers.InitializeEmptyPlayfield(5, 5);
            horizontalWalls_initial = TestHelpers.InitializeEmptyHorizontalWalls(5, 5);
            verticalWalls_initial = TestHelpers.InitializeEmptyVerticalWalls(5, 5);
            startingPositions_initial = new List<Position> { new Position(0, 0) };
            holes_initial = new List<Teleporter>();

            centaur_initial = new Centaur(-1, -1, new List<CentaurStep>());

            board_initial = new BoardState(playfield_initial, 
                horizontalWalls_initial, 
                verticalWalls_initial, 
                holes_initial,
                centaur_initial, 
                startingPositions_initial);

            player1_initial = new Player() { Name = "Geir" };
            players_initial = new List<Player>();
            players_initial.Add(player1_initial);

            game = new GameState(board_initial, players_initial);
        }

        // Accessors for the finished GameState object
        Player Player1 { get { return game.Players[0]; } }
        Player Player2 { get { return game.Players[1]; } }
        Player Player3 { get { return game.Players[2]; } }
        BoardState Board { get { return game.Board; } }
        Centaur Centaur { get { return game.Board.centaur; } }

        [Test]
        public void When_starting_game_players_should_be_at_starting_positions()
        {
            startingPositions_initial = new List<Position>()
            {
                new Position(1,1),
                new Position(2,2),
                new Position(3,3)
            };
            var player2_initial = new Player();
            players_initial.Add(player2_initial);
            initializeNewGameStateFromSetupParameters(useBoardDefinedStartingPositions:true);

            Assert.That(Player1.X == 1 && Player1.Y == 1);
            Assert.That(Player2.X == 2 && Player2.Y == 2);

            var player3_initial = new Player();
            players_initial.Add(player3_initial);
            Player1.X = Player1.Y = Player2.X = Player2.Y = 0; // Reset positions for next test case
            initializeNewGameStateFromSetupParameters(useBoardDefinedStartingPositions: true);

            Assert.That(Player1.X == 1 && Player1.Y == 1);
            Assert.That(Player2.X == 2 && Player2.Y == 2);
            Assert.That(Player3.X == 3 && Player3.Y == 3);

            int initialRngSeed = 1338;
            var player4_initial = new Player();
            players_initial.Add(player4_initial);
            Player1.X = Player1.Y = Player2.X = Player2.Y = Player3.X = Player3.Y = 0;
            initializeNewGameStateFromSetupParameters(initialRngSeed, 
                useBoardDefinedStartingPositions: true);

            Assert.That(Player1.X == 1 && Player1.Y == 1);
            Assert.That(Player2.X == 2 && Player2.Y == 2);
            Assert.That(Player3.X == 3 && Player3.Y == 3);
            Assert.IsFalse(game.Players[3].X == 0 && game.Players[3].Y == 0); // Player 4 gets random position
        }

        [Test]
        public void At_end_of_turn_centaur_should_move()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(0,0,false),
                new CentaurStep(1,0,false)
            };
            centaur_initial = new Centaur(-1, -1, centaurMoves);

            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
        }

        [Test]
        public void Centaur_with_no_moves_should_remain_stationary()
        {
            var centaurMoves = new List<CentaurStep>();
            centaur_initial = new Centaur(1, 1, centaurMoves);

            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur_initial.X == 1 && centaur_initial.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(centaur_initial.X == 1 && centaur_initial.Y == 1);
        }

        [Test]
        public void Centaur_with_only_one_move_should_remain_stationary_except_for_initial_move()
        {
            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
            };
            centaur_initial = new Centaur(0, 0, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;
            initializeNewGameStateFromSetupParameters();

            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
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
            centaur_initial = new Centaur(-1, -1, centaurMoves);

            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            // Forwards
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 3 && Centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            // Backwards
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 3 && Centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 3 && Centaur.Y == 3);

            // Repeat, except with the shortest path possible
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(0,0,false),
                new CentaurStep(1,0,false),
            };
            centaur_initial = new Centaur(-1, -1, centaurMoves);

            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 0);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0);
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
            centaur_initial = new Centaur(2, 2, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;
            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 3 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 3);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 2);
            makeCurrentPlayerDoNothing();

            Assert.IsFalse(Centaur.X == 2 && Centaur.Y == 1);
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
            centaur_initial = new Centaur(0, 1, centaurMoves);
            player1_initial.X = 0;
            player1_initial.Y = 0;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(2, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 1);

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
            centaur_initial = new Centaur(1, 0, centaurMoves);
            player1_initial.X = 0;
            player1_initial.Y = 0;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 2).IsPassable = false;

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 2);
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
            centaur_initial = new Centaur(0, 1, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);

            // Second case: Up-and-down movement
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur_initial = new Centaur(1, 0, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
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
            centaur_initial = new Centaur(0, 1, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            game.Board.GetWallRightOf(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 2 && Centaur.Y == 1); // Centaur resumes to the right

            // Second case: Centaur is walled in when moving, and released to the left
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(2,1,false),
                new CentaurStep(1,1,false),
                new CentaurStep(0,1,false)
            };
            centaur_initial = new Centaur(0, 1, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;
            game.Board.GetWallLeftOf(1, 1).IsPassable = true; // Reset walls for this
            game.Board.GetWallRightOf(1, 1).IsPassable = true;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallLeftOf(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallRightOf(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            game.Board.GetWallLeftOf(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 0 && Centaur.Y == 1); // Centaur resumes to the left
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);

            // Third case: Centaur is walled in when moving, and released upwards
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur_initial = new Centaur(1, 0, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            game.Board.GetWallAbove(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 0); // Centaur resumes up

            // Fourth case: Centaur is walled in when moving, and released downwards
            centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,1,false),
                new CentaurStep(1,2,false),
                new CentaurStep(1,1,false),
                new CentaurStep(1,0,false)
            };
            centaur_initial = new Centaur(1, 0, centaurMoves);
            player1_initial.X = 4;
            player1_initial.Y = 4;
            game.Board.GetWallAbove(1, 1).IsPassable = true; // Reset walls for this
            game.Board.GetWallBelow(1, 1).IsPassable = true;

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            game.Board.GetWallAbove(1, 1).IsPassable = false; // Centaur is at (1,1)
            game.Board.GetWallBelow(1, 1).IsPassable = false;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 1);
            game.Board.GetWallBelow(1, 1).IsPassable = true;
            makeCurrentPlayerDoNothing();
            Assert.IsTrue(Centaur.X == 1 && Centaur.Y == 2); // Centaur resumes down
        }

        [Test]
        public void When_moving_to_centaur_player_should_die()
        {
            var centaurMoves = new List<CentaurStep>();
            centaur_initial = new Centaur(1, 1, centaurMoves);

            player1_initial.X = 2;
            player1_initial.Y = 1;
            player1_initial.IsAlive = true;

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
            centaur_initial = new Centaur(-1, -1, centaurMoves);

            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.IsAlive = true;

            var player2 = new Player() { Name = "Nemesis", IsAlive = true };
            players_initial.Add(player2);
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
            centaur_initial = new Centaur(2, 2, centaurMoves);

            player1_initial.X = 0;
            player1_initial.Y = 2;

            var player2 = new Player() { Name = "player2", X = 2, Y = 0 };
            var player3 = new Player() { Name = "player2", X = 2, Y = 4 };
            var player4 = new Player() { Name = "player2", X = 4, Y = 2 };

            players_initial.Add(player2);
            players_initial.Add(player3);
            players_initial.Add(player4);

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
            centaur_initial = new Centaur(1, 1, centaurMoves);

            player1_initial.X = 2;
            player1_initial.Y = 0;

            var player2 = new Player() { Name = "player2", X = 4, Y = 1 };
            var player3 = new Player() { Name = "player3", X = 3, Y = 3 };
            var player4 = new Player() { Name = "player4", X = 1, Y = 2 };

            players_initial.Add(player2);
            players_initial.Add(player3);
            players_initial.Add(player4);

            initializeNewGameStateFromSetupParameters();

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(Player1.Name));

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(Player2.Name));

            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            makeCurrentPlayerDoNothing();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(message.Contains("clop"));
            Assert.IsTrue(message.Contains(Player3.Name));

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
            player1_initial.X = 3;
            player1_initial.Y = 2;

            var teleporter3 = new Teleporter(2, null, 4, 3);
            var teleporter2 = new Teleporter(1, teleporter3, 4, 4);
            var teleporter1 = new Teleporter(0, teleporter2, 3, 3);
            teleporter3.NextHole = teleporter1;
            holes_initial.Add(teleporter1);
            holes_initial.Add(teleporter2);
            holes_initial.Add(teleporter3);
            playfield_initial[3, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter1);
            playfield_initial[4, 4] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter2);
            playfield_initial[4, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter3);
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveDown);
            Assert.That(Player1.X == 4 && Player1.Y == 4);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(Player1.X == 4 && Player1.Y == 4);

            game.PerformMove(MoveType.MoveUp);
            Assert.That(Player1.X == 3 && Player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(Player1.X == 3 && Player1.Y == 3);
        }

        [Test]
        public void When_jumping_into_hole_player_should_move_to_next_hole_but_not_when_waiting()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;

            var teleporter3 = new Teleporter(2, null, 4, 3);
            var teleporter2 = new Teleporter(1, teleporter3, 4, 4);
            var teleporter1 = new Teleporter(0, teleporter2, 3, 3);
            teleporter3.NextHole = teleporter1;
            holes_initial.Add(teleporter1);
            holes_initial.Add(teleporter2);
            holes_initial.Add(teleporter3);
            playfield_initial[3, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter1);
            playfield_initial[4, 4] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter2);
            playfield_initial[4, 3] = new PlayfieldSquare(SquareType.Teleporter, 0, teleporter3);
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            Assert.That(Player1.X == 3 && Player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(Player1.X == 3 && Player1.Y == 3);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(Player1.X == 4 && Player1.Y == 4);
            game.PerformMove(MoveType.FallThroughHole); // Invalid operation, should have no effect
            Assert.That(Player1.X == 4 && Player1.Y == 4);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(Player1.X == 4 && Player1.Y == 3);
            game.PerformMove(MoveType.ThrowGrenadeDown);
            Assert.That(Player1.X == 4 && Player1.Y == 3);

            game.PerformMove(MoveType.FallThroughHole);
            Assert.That(Player1.X == 3 && Player1.Y == 3);
            game.PerformMove(MoveType.DoNothing);
            Assert.That(Player1.X == 3 && Player1.Y == 3);
        }

        [Test]
        public void When_starting_game_players_are_alive()
        {
            var player2_initial = new Player() { Name = "Nemesis" };
            players_initial.Add(player2_initial);
            initializeNewGameStateFromSetupParameters();

            Assert.IsTrue(Player1.IsAlive);
            Assert.IsTrue(Player2.IsAlive);
        }

        [Test]
        public void When_shot_player_should_die()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 1;

            var player2_initial = new Player() { Name = "Nemesis", IsAlive = true };
            players_initial.Add(player2_initial);
            player2_initial.X = 2;
            player2_initial.Y = 0;

            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(Player2.IsAlive);
            Assert.AreEqual(Player1.NumArrows, 0, "Should spend arrows when shooting.");

            player2_initial.X = 4;
            player2_initial.Y = 2;
            player2_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(Player2.IsAlive);
            Assert.AreEqual(Player1.NumArrows, 0, "Should spend arrows when shooting.");

            player2_initial.X = 2;
            player2_initial.Y = 4;
            player2_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(Player2.IsAlive);
            Assert.AreEqual(Player1.NumArrows, 0, "Should spend arrows when shooting.");

            player2_initial.X = 0;
            player2_initial.Y = 2;
            player2_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(Player2.IsAlive);
            Assert.AreEqual(Player1.NumArrows, 0, "Should spend arrows when shooting.");

            player2_initial.X = player1_initial.X;
            player2_initial.Y = player1_initial.Y;
            player2_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsFalse(Player2.IsAlive);
            Assert.AreEqual(Player1.NumArrows, 0, "Should spend arrows when shooting.");

        }

        [Test]
        public void When_shooting_at_same_square_dead_players_should_not_interfere()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 1;

            var player2_initial = new Player() { Name = "Nemesis", IsAlive = true };
            players_initial.Add(player2_initial);
            player2_initial.X = 2;
            player2_initial.Y = 2;

            var player3_initial = new Player() { Name = "Ghost", IsAlive = false };
            players_initial.Add(player3_initial);
            player3_initial.X = 2;
            player3_initial.Y = 2;

            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.False(Player2.IsAlive);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

            // Ghost attempts illegal move: Shooting at the same square
            game.PerformMove(MoveType.FireAtSameSquare);
        }

        [Test]
        public void When_shot_through_wall_player_should_survive()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 4;
            buildWallsAroundSquare(0, 2);
            buildWallsAroundSquare(4, 2);
            buildWallsAroundSquare(2, 4);
            buildWallsAroundSquare(2, 0);

            var nemesis = new Player() { Name = "Nemesis", IsAlive = true };
            players_initial.Add(nemesis);
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
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 4;

            var player2_initial = new Player() { Name = "Nemesis", IsAlive = true };
            var player3_initial = new Player() { Name = "InnocentBystander", IsAlive = true };
            players_initial.Add(player2_initial);
            players_initial.Add(player3_initial);

            player2_initial.X = 2;
            player2_initial.Y = 1;
            player3_initial.X = 2;
            player3_initial.Y = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(Player2.IsAlive);
            Assert.IsTrue(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 3;
            Player2.Y = 2;
            Player3.X = 4;
            Player3.Y = 2;
            Player2.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(Player2.IsAlive);
            Assert.IsTrue(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 2;
            Player2.Y = 3;
            Player3.X = 2;
            Player3.Y = 4;
            Player2.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(Player2.IsAlive);
            Assert.IsTrue(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 1;
            Player2.Y = 2;
            Player3.X = 0;
            Player3.Y = 2;
            Player2.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(Player2.IsAlive);
            Assert.IsTrue(Player3.IsAlive);
        }

        [Test]
        public void Arrows_should_pass_through_dead_players()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 4;

            var player2_initial = new Player() { Name = "DeadPlayer", IsAlive = false };
            var player3_initial = new Player() { Name = "Victim", IsAlive = true };
            players_initial.Add(player2_initial);
            players_initial.Add(player3_initial);
            player2_initial.X = 2;
            player2_initial.Y = 1;
            player3_initial.X = 2;
            player3_initial.Y = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 3;
            Player2.Y = 2;
            Player3.X = 4;
            Player3.Y = 2;
            Player3.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 2;
            Player2.Y = 3;
            Player3.X = 2;
            Player3.Y = 4;
            Player3.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(Player3.IsAlive);

            initializeNewGameStateFromSetupParameters();
            Player2.X = 1;
            Player2.Y = 2;
            Player3.X = 0;
            Player3.Y = 2;
            Player3.IsAlive = true;
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(Player3.IsAlive);
        }

        [Test]
        public void When_shooting_at_exit_arrow_should_hit_nothing()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 4;
            initializeNewGameStateFromSetupParameters();

            Board.GetWallAbove(2, 0).IsExit = true;
            Board.GetWallAbove(2, 0).IsPassable = true;
            Board.GetWallAbove(2, 0).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.FireUp);
            Assert.IsTrue(message.Contains("missed"));

            Board.GetWallRightOf(4, 2).IsExit = true;
            Board.GetWallRightOf(4, 2).IsPassable = true;
            Board.GetWallRightOf(4, 2).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireRight);
            Assert.IsTrue(message.Contains("missed"));

            Board.GetWallBelow(2, 4).IsExit = true;
            Board.GetWallBelow(2, 4).IsPassable = true;
            Board.GetWallBelow(2, 4).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireDown);
            Assert.IsTrue(message.Contains("missed"));

            Board.GetWallLeftOf(0, 2).IsExit = true;
            Board.GetWallLeftOf(0, 2).IsPassable = true;
            Board.GetWallLeftOf(0, 2).IsExterior = true;
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireLeft);
            Assert.IsTrue(message.Contains("missed"));

            Assert.AreEqual(Player1.NumArrows, 0);
        }

        [Test]
        public void When_shooting_centaur_should_see_message_and_miss_player_behind_centaur()
        {
            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 4;

            var innocentBystander = new Player() { Name = "InnocentBystander", IsAlive = true };
            players_initial.Add(innocentBystander);

            centaur_initial = new Centaur(2, 1, new List<CentaurStep>());
            innocentBystander.X = 2;
            innocentBystander.Y = 0;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.FireUp);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur_initial = new Centaur(3, 2, new List<CentaurStep>());
            innocentBystander.X = 4;
            innocentBystander.Y = 2;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireRight);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur_initial = new Centaur(2, 3, new List<CentaurStep>());
            innocentBystander.X = 2;
            innocentBystander.Y = 4;
            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.FireDown);
            Assert.IsTrue(message.Contains("centaur"));
            Assert.IsTrue(innocentBystander.IsAlive);

            centaur_initial = new Centaur(1, 2, new List<CentaurStep>());
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

            player1_initial.X = 2;
            player1_initial.Y = 2;
            player1_initial.NumArrows = 5;

            var player2_initial = new Player() { Name = "Shrodinger1", IsAlive = true };
            var player3_initial = new Player() { Name = "Shrodinger2", IsAlive = true };
            var player4_initial = new Player() { Name = "Shrodinger3", IsAlive = true };
            var player5_initial = new Player() { Name = "Shrodinger4", IsAlive = true };

            // It turns out that for this particular RNG seed, PLAYER 3 is the unlucky victim.

            players_initial.Add(player2_initial);
            players_initial.Add(player3_initial);
            players_initial.Add(player4_initial);
            players_initial.Add(player5_initial);

            player2_initial.X = player3_initial.X = player4_initial.X = player5_initial.X = 2;
            player2_initial.Y = player3_initial.Y = player4_initial.Y = player5_initial.Y = 0;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireUp);
            Assert.IsFalse(Player3.IsAlive);

            player2_initial.X = player3_initial.X = player4_initial.X = player5_initial.X = 4;
            player2_initial.Y = player3_initial.Y = player4_initial.Y = player5_initial.Y = 2;
            player2_initial.IsAlive = player3_initial.IsAlive = player4_initial.IsAlive = player5_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);
            Assert.IsFalse(Player3.IsAlive);

            player2_initial.X = player3_initial.X = player4_initial.X = player5_initial.X = 2;
            player2_initial.Y = player3_initial.Y = player4_initial.Y = player5_initial.Y = 4;
            player2_initial.IsAlive = player3_initial.IsAlive = player4_initial.IsAlive = player5_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireDown);
            Assert.IsFalse(Player3.IsAlive);

            player2_initial.X = player3_initial.X = player4_initial.X = player5_initial.X = 0;
            player2_initial.Y = player3_initial.Y = player4_initial.Y = player5_initial.Y = 2;
            player2_initial.IsAlive = player3_initial.IsAlive = player4_initial.IsAlive = player5_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.IsFalse(Player3.IsAlive);

            player2_initial.X = player3_initial.X = player4_initial.X = player5_initial.X = player1_initial.X;
            player2_initial.Y = player3_initial.Y = player4_initial.Y = player5_initial.Y = player1_initial.Y;
            player2_initial.IsAlive = player3_initial.IsAlive = player4_initial.IsAlive = player5_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters(initialRngSeed);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsFalse(Player3.IsAlive);
        }

        [Test]
        public void When_player_uses_nonexistent_gear_nothing_should_happen()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.NumArrows = 0;
            player1_initial.NumGrenades = 0;

            var player2 = new Player();
            player2.X = 0;
            player2.Y = 3;
            players_initial.Add(player2);

            board_initial.GetWallAbove(3, 3).IsPassable = false;
            board_initial.GetWallAbove(3, 3).HasHamster = true;
            board_initial.GetWallRightOf(3, 3).IsPassable = false;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayUp);
            Assert.True(Board.GetWallAbove(3, 3).HasHamster);

            makeCurrentPlayerDoNothing(); // Skip player 2

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterRight);
            Assert.IsFalse(Board.GetWallRightOf(3, 3).HasHamster);

            makeCurrentPlayerDoNothing(); // Skip player 2

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireLeft);
            Assert.True(Player2.IsAlive);

            makeCurrentPlayerDoNothing(); // Skip player 2

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeRight);
            Assert.IsFalse(Board.GetWallRightOf(3, 3).IsPassable);

            makeCurrentPlayerDoNothing(); // Skip player 2

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallDown);
            Assert.IsTrue(Board.GetWallBelow(3, 3).IsPassable);
        }

        [Test]
        public void When_visiting_ammo_storage_player_should_replenish_weapons()
        {
            playfield_initial[3, 4] = new PlayfieldSquare(SquareType.AmmoStorage, 0);

            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.IsAlive = true;
            player1_initial.NumArrows = 0;
            player1_initial.NumGrenades = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(Player1.NumArrows, Player.ArrowCapacity);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity);

        }

        [Test]
        public void When_visiting_hamster_storage_player_should_replenish_hamster_gear()
        {
            playfield_initial[3, 4] = new PlayfieldSquare(SquareType.HamsterStorage, 0);

            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.IsAlive = true;
            player1_initial.NumHamsters = 0;
            player1_initial.NumHamsterSprays = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(Player1.NumHamsters, Player.HamsterCapacity);
            Assert.AreEqual(Player1.NumHamsterSprays, Player.HamsterSprayCapacity);
        }

        [Test]
        public void When_visiting_fitness_studio_dead_player_should_see_it_and_be_resurrected()
        {
            playfield_initial[3, 4] = new PlayfieldSquare(SquareType.FitnessStudio, 0);

            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.IsAlive = false;
            initializeNewGameStateFromSetupParameters();

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsTrue(Player1.IsAlive);
            Assert.IsTrue(message.Contains("fitness studio"));
        }

        [Test]
        public void When_visiting_fitness_studio_live_player_should_see_empty_room()
        {
            playfield_initial[3, 4] = new PlayfieldSquare(SquareType.FitnessStudio, 0);

            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.IsAlive = true;
            initializeNewGameStateFromSetupParameters();

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsFalse(message.Contains("fitness studio"));
        }

        [Test]
        public void When_standing_in_storage_and_using_consumables_should_immediately_replenish_consumables()
        {
            // Test for each of "fire, then wait" and "fire as part of movement action".
            // Test for each of the consumable locations.

            Assert.Fail("Not implemented");
        }

        [Test]
        public void Dead_player_should_not_replenish_consumables_when_visiting_storage()
        {
            playfield_initial[2, 1] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            playfield_initial[3, 1] = new PlayfieldSquare(SquareType.CementStorage, 0);
            playfield_initial[4, 1] = new PlayfieldSquare(SquareType.HamsterStorage, 0);

            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumArrows = 0;
            player1_initial.NumGrenades = 0;
            player1_initial.NumHamsters = 0;
            player1_initial.NumHamsterSprays = 0;
            player1_initial.NumCement = 0;
            player1_initial.IsAlive = false;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.MoveRight);

            Assert.AreEqual(Player1.NumArrows, 0);
            Assert.AreEqual(Player1.NumGrenades, 0);
            Assert.AreEqual(Player1.NumHamsters, 0);
            Assert.AreEqual(Player1.NumHamsterSprays, 0);
            Assert.AreEqual(Player1.NumCement, 0);
        }

        [Test]
        public void Player_should_drop_treasure_and_consumables_when_killed()
        {
            player1_initial.X = 0;
            player1_initial.Y = 0;
            player1_initial.NumArrows = 1;
            board_initial.GetPlayfieldSquareOf(1, 0).NumTreasures = 0;

            var player2_initial = new Player()
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
            players_initial.Add(player2_initial);
            player2_initial.X = 1;
            player2_initial.Y = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.FireRight);

            Assert.IsFalse(Player2.CarriesTreasure);
            Assert.AreEqual(Player2.NumArrows, 0);
            Assert.AreEqual(Player2.NumGrenades, 0);
            Assert.AreEqual(Player2.NumHamsters, 0);
            Assert.AreEqual(Player2.NumHamsterSprays, 0);
            Assert.AreEqual(Player2.NumCement, 0);
            Assert.AreEqual(Board.GetPlayfieldSquareOf(Player2).NumTreasures, 1);
        }

        [Test]
        public void When_visiting_cement_storage_player_should_replenish_cement()
        {
            playfield_initial[3, 4] = new PlayfieldSquare(SquareType.CementStorage, 0);
            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.IsAlive = true;
            player1_initial.NumCement = 0;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveDown);

            Assert.AreEqual(Player1.NumCement, Player.CementCapacity);
        }

        [Test]
        public void When_hitting_wall_player_should_not_move()
        {
            buildWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 1);
            Assert.AreEqual(Player1.Y, 1);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 1);
            Assert.AreEqual(Player1.Y, 1);

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 1);
            Assert.AreEqual(Player1.Y, 1);

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 1);
            Assert.AreEqual(Player1.Y, 1);
        }

        [Test]
        public void When_moving_in_open_area_player_should_move()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 3);
            Assert.AreEqual(Player1.Y, 2);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 4);
            Assert.AreEqual(Player1.Y, 2);

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 4);
            Assert.AreEqual(Player1.Y, 3);

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            Assert.AreEqual(Player1.X, 3);
            Assert.AreEqual(Player1.Y, 3);
        }

        [Test]
        public void When_placing_hamster_hamster_should_be_placed()
        {
            buildWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumHamsters = 4;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterUp);

            Assert.IsTrue(Board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterRight);

            Assert.IsTrue(Board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterDown);

            Assert.IsTrue(Board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterLeft);

            Assert.IsTrue(Board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(Player1.NumHamsters, 0, "When placing hamster, hamster count should decrease");
        }

        [Test]
        public void When_placing_hamster_on_hamstered_wall_should_yield_no_result()
        {
            buildHamsteredWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumHamsters = 4;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterUp);

            Assert.IsTrue(Board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterRight);

            Assert.IsTrue(Board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterDown);

            Assert.IsTrue(Board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.PlaceHamsterLeft);

            Assert.IsTrue(Board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(Player1.NumHamsters, 4);
        }

        /// <summary>
        /// If as a followup action e.g. constructing a wall where there is a wall, the
        /// turn should pass to the next player to avoid exploiting this for quick exploration.
        /// </summary>
        [Test]
        public void Blocked_followup_actions_should_execute()
        {
            player1_initial.X = 0;
            player1_initial.Y = 0;
            player1_initial.NumHamsters = 3;
            player1_initial.NumHamsterSprays = 3;
            player1_initial.NumGrenades = 3;
            player1_initial.NumCement = 3;
            // Performing all tests against an exit, to also cover the potential case of invalid wall status
            horizontalWalls_initial[0, 0].IsExit = true;
            horizontalWalls_initial[0, 0].IsPassable = true;

            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.PlaceHamsterUp);
            Assert.AreEqual(Player1.NumHamsters, 2);
            Board.GetWallAbove(0, 0).IsPassable = false;
            game.PerformMove(MoveType.PlaceHamsterUp);
            Assert.AreEqual(Player1.NumHamsters, 1);
            Assert.That(Board.GetWallAbove(0, 0).HasHamster);
            game.PerformMove(MoveType.PlaceHamsterUp);
            Assert.AreEqual(Player1.NumHamsters, 1);
            Assert.That(Board.GetWallAbove(0, 0).IsExit);

            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.BuildWallUp);
            Assert.AreEqual(Player1.NumCement, 2);
            Assert.False(Board.GetWallAbove(0, 0).IsPassable);
            game.PerformMove(MoveType.BuildWallUp);
            Assert.AreEqual(Player1.NumCement, 2);
            Assert.False(Board.GetWallAbove(0, 0).IsPassable);
            Board.GetWallAbove(0, 0).HasHamster = true;
            game.PerformMove(MoveType.BuildWallUp);
            Assert.AreEqual(Player1.NumCement, 2);
            Assert.False(Board.GetWallAbove(0, 0).IsPassable);
            Assert.That(Board.GetWallAbove(0, 0).IsExit);

            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.HamsterSprayUp);
            Assert.AreEqual(Player1.NumHamsterSprays, 2);
            Assert.False(Board.GetWallAbove(0, 0).HasHamster);
            Board.GetWallAbove(0, 0).IsPassable = false;
            Board.GetWallAbove(0, 0).HasHamster = true;
            game.PerformMove(MoveType.HamsterSprayUp);
            Assert.AreEqual(Player1.NumHamsterSprays, 1);
            Assert.False(Board.GetWallAbove(0, 0).HasHamster);
            game.PerformMove(MoveType.HamsterSprayUp);
            Assert.AreEqual(Player1.NumHamsterSprays, 0);
            Assert.That(Board.GetWallAbove(0, 0).IsExit);

            initializeNewGameStateFromSetupParameters();
            game.PerformMove(MoveType.ThrowGrenadeUp);
            Assert.AreEqual(Player1.NumGrenades, 2);
            Assert.True(Board.GetWallAbove(0, 0).IsPassable);
            Board.GetWallAbove(0, 0).IsPassable = false;
            Board.GetWallAbove(0, 0).HasHamster = true;
            game.PerformMove(MoveType.ThrowGrenadeUp);
            Assert.AreEqual(Player1.NumGrenades, 2);
            Assert.That(Board.GetWallAbove(0, 0).HasHamster);
            Assert.False(Board.GetWallAbove(0, 0).IsPassable);
            Board.GetWallAbove(0, 0).HasHamster = false;
            game.PerformMove(MoveType.ThrowGrenadeUp);
            Assert.AreEqual(Player1.NumGrenades, 1);
            Assert.That(Board.GetWallAbove(0, 0).IsPassable);
            Assert.That(Board.GetWallAbove(0, 0).IsExit);
        }

        [Test]
        public void When_spraying_hamsters_hamsters_should_die()
        {
            buildHamsteredWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumHamsterSprays = 4;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayUp);

            Assert.IsFalse(Board.GetWallAbove(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayRight);

            Assert.IsFalse(Board.GetWallRightOf(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayDown);

            Assert.IsFalse(Board.GetWallBelow(1, 1).HasHamster);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.HamsterSprayLeft);

            Assert.IsFalse(Board.GetWallLeftOf(1, 1).HasHamster);

            Assert.AreEqual(Player1.NumHamsterSprays, 0, "When spraying hamsters, should spend hamster spray");
        }

        [Test]
        public void When_constructing_wall_wall_should_be_created()
        {
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumCement = 4;
            board_initial.GetWallAbove(1, 1).IsExterior = true;
            board_initial.GetWallAbove(1, 1).IsExit = true;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallUp);

            Assert.IsFalse(Board.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallRight);

            Assert.IsFalse(Board.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallDown);

            Assert.IsFalse(Board.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallLeft);

            Assert.IsFalse(Board.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(Player1.NumCement, 0, "When using cement, cement count should decrease");
        }

        [Test]
        public void When_attempting_to_construct_wall_on_wall_should_yield_no_result()
        {
            buildWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumCement = 4;

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallUp);

            Assert.IsFalse(board_initial.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallRight);

            Assert.IsFalse(board_initial.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallDown);

            Assert.IsFalse(board_initial.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.BuildWallLeft);

            Assert.IsFalse(board_initial.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(player1_initial.NumCement, 4, "When building on existing walls, cement should not decrease");
        }

        [Test]
        public void When_blowing_up_wall_wall_should_disappear()
        {
            buildWallsAroundSquare(1, 1);
            player1_initial.X = 1;
            player1_initial.Y = 1;
            player1_initial.NumGrenades = 4;
            initializeNewGameStateFromSetupParameters();

            var expectedMessage = "You blow up the wall. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallAbove(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallRightOf(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallBelow(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallLeftOf(1, 1).IsPassable);

            Assert.AreEqual(Player1.NumGrenades, 0, "When using grenades, grenade count should decrease");
        }

        [Test]
        public void When_blowing_up_exterior_wall_should_see_message_and_no_result()
        {
            player1_initial.X = 0;
            player1_initial.Y = 0;
            player1_initial.NumGrenades = 4;
            initializeNewGameStateFromSetupParameters();

            var expectedMessage = "The grenade explodes against an exterior wall. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallAbove(Player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallLeftOf(Player1).IsPassable);

            Player1.X = 4;
            Player1.Y = 4;

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallRightOf(Player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallBelow(Player1).IsPassable);

            Assert.AreEqual(Player1.NumGrenades, 0);
        }

        [Test]
        public void When_blowing_up_hidden_exit_should_remove_wall()
        {
            player1_initial.X = 0;
            player1_initial.Y = 0;
            player1_initial.NumGrenades = 4;
            var expectedMessage = "You blow up the wall. ";

            var firstHiddenExit = board_initial.GetWallAbove(0, 0);
            var secondHiddenExit = board_initial.GetWallLeftOf(0, 0);
            var thirdHiddenExit = board_initial.GetWallRightOf(4, 4);
            var fourthHiddenExit = board_initial.GetWallBelow(4, 4);

            firstHiddenExit.IsExit = secondHiddenExit.IsExit =
                thirdHiddenExit.IsExit = fourthHiddenExit.IsExit = true;
            firstHiddenExit.IsPassable = secondHiddenExit.IsPassable =
                thirdHiddenExit.IsPassable = fourthHiddenExit.IsPassable = false;
            firstHiddenExit.IsExterior = secondHiddenExit.IsExterior =
                thirdHiddenExit.IsExterior = fourthHiddenExit.IsExterior = true;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallAbove(Player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallLeftOf(Player1).IsPassable);

            Player1.X = 4;
            Player1.Y = 4;

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallRightOf(Player1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(Board.GetWallBelow(Player1).IsPassable);
        }

        [Test]
        public void When_blowing_up_hamster_wall_should_see_message_and_no_result()
        {
            buildHamsteredWallsAroundSquare(3, 1);
            player1_initial.X = 3;
            player1_initial.Y = 1;
            player1_initial.NumGrenades = 4;
            initializeNewGameStateFromSetupParameters();

            var expectedMessage = "A hamster returns your grenade with the pin inserted. ";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallAbove(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallRightOf(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallBelow(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(Board.GetWallLeftOf(3, 1).IsPassable);

            Assert.AreEqual(Player1.NumGrenades, 4, "Hamster walls should not spend grenades");
        }

        [Test]
        public void When_visiting_treasure_player_should_take_it()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;
            board_initial.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.True(Player1.CarriesTreasure);
            Assert.AreEqual(Board.GetPlayfieldSquareOf(3, 2).NumTreasures, 1);
        }

        [Test]
        public void When_visiting_treasure_when_already_carrying_player_should_see_and_leave_it()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.CarriesTreasure = true;
            board_initial.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;
            initializeNewGameStateFromSetupParameters();

            string moveDescription = game.PerformMove(MoveType.MoveUp);

            Assert.True(moveDescription.Contains("There is treasure here"));
            Assert.True(Player1.CarriesTreasure);
            Assert.AreEqual(Board.GetPlayfieldSquareOf(3, 2).NumTreasures, 2);
        }

        [Test]
        public void When_visiting_treasure_dead_player_should_see_and_leave_it()
        {
            player1_initial.X = 3;
            player1_initial.Y = 3;
            player1_initial.CarriesTreasure = false;
            player1_initial.IsAlive = false;
            board_initial.GetPlayfieldSquareOf(3, 2).NumTreasures = 2;
            initializeNewGameStateFromSetupParameters();

            string moveDescription = game.PerformMove(MoveType.MoveUp);

            Assert.True(moveDescription.Contains("There is treasure here"));
            Assert.False(Player1.CarriesTreasure);
            Assert.AreEqual(Board.GetPlayfieldSquareOf(3, 2).NumTreasures, 2);
        }

        [Test]
        public void When_exiting_labyrinth_in_single_player_should_stay_outside_for_one_move_sequence()
        {
            player1_initial.X = 0;
            player1_initial.Y = 3;
            player1_initial.NumHamsters = 1;
            player1_initial.NumArrows = 1;
            player1_initial.NumCement = 1;
            var exit = board_initial.GetWallLeftOf(0, 3);

            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;
            initializeNewGameStateFromSetupParameters();

            var message1 = game.PerformMove(MoveType.MoveLeft);
            Assert.IsTrue(Player1.IsOutsideLabyrinth());
            var message2 = game.PerformMove(MoveType.PlaceHamsterRight); // No effect, outside
            Assert.AreEqual(Player1.NumHamsters, 1);
            Assert.IsTrue(Player1.IsOutsideLabyrinth());
            var message3 = game.PerformMove(MoveType.MoveRight);  // No effect, outside
            Assert.IsTrue(Player1.IsOutsideLabyrinth());
            Assert.IsTrue(Player1.X == -1 && Player1.Y == 3);
            var message4 = game.PerformMove(MoveType.BuildWallRight); // No effect, outside
            Assert.AreEqual(Player1.NumCement, 1);

            Assert.IsFalse(Player1.IsOutsideLabyrinth());
            game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(Player1.X == 1 && Player1.Y == 3);

            Assert.IsTrue(message1.Contains("outside"));
            Assert.IsTrue(message2.Contains("outside"));
            Assert.IsTrue(message3.Contains("outside"));
            Assert.IsTrue(message4.Contains("enter"));
        }

        [Test]
        public void When_exiting_labyrinth_player_should_return_at_end_of_second_turn()
        {
            player1_initial.X = 0;
            player1_initial.Y = 1;
            var exit = board_initial.GetWallLeftOf(0, 1);

            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;
            initializeNewGameStateFromSetupParameters();

            game.PerformMove(MoveType.MoveLeft);
            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(Player1.IsOutsideLabyrinth());
            game.PerformMove(MoveType.DoNothing);
            Assert.IsFalse(Player1.IsOutsideLabyrinth());
        }

        [Test]
        public void When_exiting_labyrinth_player_should_skip_turn_and_reenter()
        {
            var player2_initial = new Player() { Name = "Spelunker", X = 0, Y = 0 };
            players_initial.Add(player2_initial);
            player1_initial.X = 0;
            player1_initial.Y = 0;
            player2_initial.NumArrows = 1;
            initializeNewGameStateFromSetupParameters();

            var exit = Board.GetWallAbove(0, 0);
            exit.IsExterior = true;
            exit.IsPassable = true;
            exit.IsExit = true;

            game.PerformMove(MoveType.MoveUp); // Player 1 exits
            Assert.IsTrue(Player1.IsOutsideLabyrinth());
            Assert.IsTrue(Player1.X == 0 && Player1.Y == -1);
            var player1Message = game.PerformMove(MoveType.PlaceHamsterLeft);
            Assert.IsTrue(player1Message.Contains("outside"));

            var player2Message = game.PerformMove(MoveType.MoveUp); // Player 2 exits
            Assert.IsTrue(Player2.IsOutsideLabyrinth());
            Assert.IsTrue(Player2.X == 0 && Player2.Y == -1);
            Assert.IsTrue(player2Message.Contains("outside"));
            game.PerformMove(MoveType.FireAtSameSquare);
            Assert.IsTrue(Player1.IsAlive);
            Assert.AreEqual(Player2.NumArrows, 1);

            player1Message = game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(player1Message.Contains("outside"));
            Assert.IsTrue(Player1.X == 0 && Player1.Y == -1);
            player1Message = game.PerformMove(MoveType.MoveRight);
            Assert.IsTrue(player1Message.Contains("enter"));
            Assert.IsTrue(Player1.X == 0 && Player1.Y == 0);

            player2Message = game.PerformMove(MoveType.FireDown);
            Assert.AreEqual(Player2.NumArrows, 1);
            Assert.IsFalse(Player2.IsOutsideLabyrinth());
            Assert.IsTrue(Player2.X == 0 && Player2.Y == 0);
            Assert.IsTrue(player2Message.Contains("enter"));

            game.PerformMove(MoveType.MoveDown);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(Player1.X == 0 && Player1.Y == 1);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(Player2.X == 1 && Player2.Y == 0);
        }

        [Test]
        public void When_exiting_labyrinth_with_treasure_player_should_get_point()
        {
            player1_initial.X = 4;
            player1_initial.Y = 2;
            player1_initial.CarriesTreasure = true;
            player1_initial.Score = 0;
            initializeNewGameStateFromSetupParameters();

            var exit = Board.GetWallRightOf(4, 2);
            exit.IsExit = true;
            exit.IsPassable = true;
            exit.IsExterior = true;

            game.PerformMove(MoveType.MoveRight);

            Assert.IsFalse(Player1.CarriesTreasure);
            Assert.IsTrue(Player1.Score == 1);
        }

        [Test]
        public void When_exiting_labyrinth_without_treasure_player_should_get_message_but_no_points()
        {
            Player1.X = 2;
            Player1.Y = 4;
            var exit = Board.GetWallBelow(2, 4);
            exit.IsExterior = true;
            exit.IsExit = true;
            exit.IsPassable = true;

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsTrue(Player1.Score == 0);
            Assert.IsTrue(message.Contains("outside"));
        }

        [Test]
        public void When_exiting_labyrinth_player_could_be_temporarily_blocked_out()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Movement_as_followup_action_should_cause_skipped_followup_action()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Followup_action_as_movement_should_skip_movement_and_execute()
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
            startingPositions_initial = new List<Position>() { new Position(2, 3) };

            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,2,false),
                new CentaurStep(2,2,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false)
            };
            centaur_initial = new Centaur(-1, -1, centaurMoves);
            board_initial.PlayfieldGrid[2, 2] = new PlayfieldSquare(SquareType.CementStorage, numTreasures: 1);
            board_initial.PlayfieldGrid[0, 2] = new PlayfieldSquare(SquareType.FitnessStudio, 0);
            board_initial.PlayfieldGrid[0, 1] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            board_initial.GetWallAbove(1, 1).IsPassable = false;
            initializeNewGameStateFromSetupParameters(useBoardDefinedStartingPositions:true);

            // Undo back to first move and beyond. Should return game state to before first move.
            game.PerformMove(MoveType.MoveUp); // Receives cement
            game.PerformMove(MoveType.BuildWallRight);
            game.PerformMove(MoveType.MoveLeft); // Hit centaur and died
            
            game.UndoPreviousMove();
            Assert.True(Player1.IsAlive);
            Assert.True(Player1.X == 2 && Player1.Y == 2);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);
            game.UndoPreviousMove();
            Assert.True(Board.GetWallRightOf(2, 2).IsPassable);
            Assert.AreEqual(Player1.NumCement, Player.CementCapacity);
            Assert.True(Centaur.X == -1 && Centaur.Y == -1);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);
            game.UndoPreviousMove();
            Assert.True(Player1.X == 2 && Player1.Y == 3);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);
            Assert.AreEqual(Player1.NumCement, 0);
            game.UndoPreviousMove(); // Try to undo past the initial game state
            Assert.True(Centaur.X == -1 && Centaur.Y == -1);
            Assert.True(Player1.X == 2 && Player1.Y == 3);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);
            Assert.AreEqual(Player1.NumCement, 0);

            game.RedoNextMove(); // Redid "move up"
            Assert.True(Board.GetWallRightOf(2, 2).IsPassable);
            Assert.AreEqual(Player1.NumCement, Player.CementCapacity);
            Assert.True(Centaur.X == -1 && Centaur.Y == -1);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);
            game.RedoNextMove(); // Redid "build wall right"
            Assert.False(Board.GetWallRightOf(2, 2).IsPassable);
            Assert.AreEqual(Player1.NumCement, Player.CementCapacity - 1);
            Assert.True(Centaur.X == 1 && Centaur.Y == 2);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);
            game.RedoNextMove(); // Redid "Walk left"
            Assert.True(Player1.X == 1 && Player1.Y == 2);
            Assert.False(Player1.IsAlive);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);
            game.RedoNextMove(); // Try to redo past the final redo state
            Assert.True(Player1.X == 1 && Player1.Y == 2);
            Assert.False(Player1.IsAlive);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);
            game.RedoNextMove(); // Nothing should happen
            Assert.False(game.CanRedo());
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);

            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.MoveLeft); // Alive again
            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.MoveUp); // Gets more ammo
            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.MoveRight);
            game.PerformMove(MoveType.FireRight);

            // Movement was automatically executed as "stand still" before followup action
            game.PerformMove(MoveType.ThrowGrenadeUp);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.MoveRight); // Nothing happens

            game.UndoPreviousMove(); // Undid "attempt to move right on followup" (do nothing)
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            game.UndoPreviousMove(); // Undid "do nothing"
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity-1);
            Assert.True(Board.GetWallAbove(1, 1).IsPassable);
            game.UndoPreviousMove(); // Undid "Grenade up"
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.False(Board.GetWallAbove(1, 1).IsPassable);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity);
            game.UndoPreviousMove(); // Undid implicit "do nothing" move
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumArrows, Player.ArrowCapacity - 1);
            game.UndoPreviousMove(); // Undid "Fire right"
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumArrows, Player.ArrowCapacity);
            game.UndoPreviousMove(); // Undid "Move right"
            Assert.True(Player1.X == 0 && Player1.Y == 1);

            game.UndoPreviousMove(); // Undid "Do nothing"
            game.UndoPreviousMove(); // Undid "Move up"

            Assert.True(Player1.X == 0 && Player1.Y == 2);

            game.UndoPreviousMove();
            Assert.True(Player1.IsAlive);
            game.UndoPreviousMove();
            Assert.False(Player1.IsAlive);
            game.RedoNextMove();
            game.RedoNextMove();
            game.RedoNextMove();
            game.RedoNextMove();
            game.RedoNextMove();

            game.RedoNextMove(); 
            game.RedoNextMove(); // Redid to immediately before "Grenade up"

            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity);
            Assert.False(Board.GetWallAbove(1, 1).IsPassable);
            game.RedoNextMove(); // Redid "Grenade up"
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity - 1);
            Assert.True(Board.GetWallAbove(1, 1).IsPassable);
            game.RedoNextMove();
            game.RedoNextMove();
            game.RedoNextMove(); // Attempted to redo past the head of the queue
            Assert.True(Player1.X == 1 && Player1.Y == 1);
            Assert.AreEqual(Player1.NumGrenades, Player.GrenadeCapacity - 1);
            Assert.True(Board.GetWallAbove(1, 1).IsPassable);
        }

        [Test]
        public void Redo_state_is_removed_after_moving()
        {
            startingPositions_initial = new List<Position>() { new Position(2, 3) };

            var centaurMoves = new List<CentaurStep>()
            {
                new CentaurStep(1,2,false),
                new CentaurStep(2,2,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false),
                new CentaurStep(4,4,false)
            };
            centaur_initial = new Centaur(-1, -1, centaurMoves);
            board_initial.PlayfieldGrid[2, 2] = new PlayfieldSquare(SquareType.CementStorage, numTreasures: 1);
            board_initial.PlayfieldGrid[0, 2] = new PlayfieldSquare(SquareType.FitnessStudio, 0);
            board_initial.PlayfieldGrid[0, 1] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            board_initial.GetWallAbove(1, 1).IsPassable = false;
            initializeNewGameStateFromSetupParameters(useBoardDefinedStartingPositions: true);

            game.PerformMove(MoveType.MoveUp); // Receives cement
            game.PerformMove(MoveType.BuildWallRight);
            game.PerformMove(MoveType.MoveLeft); // Hit centaur and died

            game.UndoPreviousMove(); // Alive again
            Assert.True(Player1.IsAlive);
            Assert.True(Player1.X == 2 && Player1.Y == 2);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);

            game.PerformMove(MoveType.MoveUp); // Performs move, should clear redo buffer.
            Assert.False(game.CanRedo());
            Assert.True(Player1.IsAlive);
            Assert.True(Player1.X == 2 && Player1.Y == 1);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);

            game.RedoNextMove(); // Should have no effect
            Assert.False(game.CanRedo());
            Assert.True(Player1.IsAlive);
            Assert.True(Player1.X == 2 && Player1.Y == 1);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectFollowupAction);

            game.PerformMove(MoveType.DoNothing);

            game.PerformMove(MoveType.BuildWallDown); // Implicitly performs a "do nothing" move first
            Assert.False(Board.GetWallBelow(2, 1).IsPassable);

            game.UndoPreviousMove();
            game.UndoPreviousMove();
            Assert.True(Board.GetWallBelow(2, 1).IsPassable);
            game.UndoPreviousMove();
            game.UndoPreviousMove(); // Undid "move up"
            Assert.True(Player1.IsAlive);
            Assert.True(Player1.X == 2 && Player1.Y == 2);
            Assert.AreEqual(game.CurrentTurnPhase, TurnPhase.SelectMainAction);

            game.RedoNextMove(); // Redid "move up"
            Assert.True(Player1.X == 2 && Player1.Y == 1);
            Assert.True(game.CanRedo());
            game.PerformMove(MoveType.BuildWallRight);
            Assert.False(game.CanRedo());
            Assert.False(Board.GetWallRightOf(2, 1).IsPassable);
            game.UndoPreviousMove();
            game.RedoNextMove();
            Assert.False(game.CanRedo());
            game.RedoNextMove(); // Ensure no crash due to moving beyond end-of-buffer
        }

        // Helper methods for creating data to test.

        private void buildWallsAroundSquare(int x, int y)
        {
            board_initial.GetWallAbove(x, y).IsPassable = false;
            board_initial.GetWallRightOf(x, y).IsPassable = false;
            board_initial.GetWallBelow(x, y).IsPassable = false;
            board_initial.GetWallLeftOf(x, y).IsPassable = false;
        }

        private void buildHamsteredWallsAroundSquare(int x, int y)
        {
            board_initial.GetWallAbove(x, y).IsPassable = false;
            board_initial.GetWallRightOf(x, y).IsPassable = false;
            board_initial.GetWallBelow(x, y).IsPassable = false;
            board_initial.GetWallLeftOf(x, y).IsPassable = false;

            board_initial.GetWallAbove(x, y).HasHamster = true;
            board_initial.GetWallRightOf(x, y).HasHamster = true;
            board_initial.GetWallBelow(x, y).HasHamster = true;
            board_initial.GetWallLeftOf(x, y).HasHamster = true;
        }

        private void makeCurrentPlayerDoNothing()
        {
            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
        }

        private void initializeNewGameStateFromSetupParameters(bool useBoardDefinedStartingPositions = false)
        {
            var playerStatePreInitialization = HelperMethods.DeepClone(players_initial);

            board_initial = new BoardState(playfield_initial, horizontalWalls_initial, verticalWalls_initial, holes_initial,
                centaur_initial, startingPositions_initial);
            game = new GameState(board_initial, players_initial);

            if (!useBoardDefinedStartingPositions)
            {
                for (int i = 0; i < game.Players.Count; i++)
                {
                    // Preserve the player positions that were defined in the test
                    game.Players[i].X = playerStatePreInitialization[i].X;
                    game.Players[i].Y = playerStatePreInitialization[i].Y;
                }
            }
        }

        private void initializeNewGameStateFromSetupParameters(int withInitialRngSeed,
            bool useBoardDefinedStartingPositions = false)
        {
            var playerStatePreInitialization = HelperMethods.DeepClone(players_initial);

            board_initial = new BoardState(playfield_initial, horizontalWalls_initial, verticalWalls_initial, holes_initial,
                centaur_initial, startingPositions_initial);
            game = new GameState(board_initial, players_initial, withInitialRngSeed);

            if (!useBoardDefinedStartingPositions)
            {
                for (int i = 0; i < game.Players.Count; i++)
                {
                    // Preserve the player positions that were defined in the test
                    game.Players[i].X = playerStatePreInitialization[i].X;
                    game.Players[i].Y = playerStatePreInitialization[i].Y;
                }
            }
        }
    }
}