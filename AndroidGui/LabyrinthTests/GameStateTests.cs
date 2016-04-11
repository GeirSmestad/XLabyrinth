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

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

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

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

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

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

            Assert.IsTrue(centaur.X == 1 && centaur.Y == 1);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);

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

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 3);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            // Backwards

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 3 && centaur.Y == 3);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
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

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.Board.centaur.ReverseDirection();

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 0 && centaur.Y == 0);

            game.PerformMove(MoveType.DoNothing);
            game.PerformMove(MoveType.DoNothing);
            Assert.IsTrue(centaur.X == 1 && centaur.Y == 0);
        }

        [Test]
        public void Centaur_should_move_through_wall_when_explicitly_specified()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_blocked_by_wall_centaur_should_reverse()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_walled_in_centaur_should_stop_moving()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_released_walled_in_centaur_should_start_moving_again()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_moving_to_centaur_player_should_die()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_hit_by_centaur_player_should_die()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_player_moves_near_centaur_should_print_clopclop()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_centaur_moves_near_player_should_print_clopclop()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_moving_to_hole_player_should_move_to_next_hole()
        {
            // TODO: Ensure that tests complete both movement and followup, and that
            // the player is only moved to the next hole and not two holes up.
            // There is room for subtle bugs in this functionality since teleporters
            // can be entered in many ways.
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_waiting_in_hole_player_should_move_to_next_hole()
        {
            Assert.Fail("Not implemented");
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
        public void When_exiting_maze_player_should_skip_turn_and_reenter()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_exiting_maze_with_treasure_player_should_get_point()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_exiting_maze_without_treasure_player_should_get_message_but_no_points()
        {
            Assert.Fail("Not implemented");
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
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            game = new GameState(board, players);
        }

        private void initializeNewGameStateFromSetupParameters(int withInitialRngSeed)
        {
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            game = new GameState(board, players, withInitialRngSeed);
        }
    }
}