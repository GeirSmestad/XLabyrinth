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

        [SetUp]
        public void SetUp()
        {
            // Initialize an empty board. You can rebuild it with arbitrary content in each test.
            playfield = initializeEmptyPlayfield(5, 5);
            horizontalWalls = initializeEmptyHorizontalWalls(5, 5);
            verticalWalls = initializeEmptyVerticalWalls(5, 5);
            startingPositions = new List<Position> { new Position(0, 0) };
            holes = new List<Teleporter>();

            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes, 
                centaur, startingPositions);

            player1 = new Player() { Name = "Geir" };
            players = new List<Player>();
            players.Add(player1);

            game = new GameState(board, players);
        }

        // Game state tests

        [Test]
        public void At_end_of_turn_centaur_should_move()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_blocked_by_wall_centaur_should_reverse()
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
        public void When_near_centaur_should_print_clopclop()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_moving_to_hole_player_should_move_to_next_hole()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_waiting_in_hole_player_should_move_to_next_hole()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Random_events_in_identical_game_states_should_resolve_identically()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_shot_player_should_die()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_shot_through_wall_player_should_survive()
        {
            Assert.Fail("Not implemented");
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
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes, 
                centaur, startingPositions);
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
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
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
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = false;
            var expectedMessage = "You enter the fitness studio and return to life! ";

            var message = game.PerformMove(MoveType.MoveDown);

            Assert.IsTrue(player1.IsAlive);
            Assert.AreEqual(message, expectedMessage);
        }

        [Test]
        public void When_visiting_fitness_studio_live_player_should_see_empty_room()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.FitnessStudio, 0);
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
            player1.X = 3;
            player1.Y = 3;
            player1.IsAlive = true;

            var message = game.PerformMove(MoveType.MoveDown);

            // TODO: Test this to ensure that the contains method works as expected
            Assert.IsFalse(message.Contains("fitness")); 
        }
        
        [Test]
        public void Dead_player_should_not_replenish_consumables_when_visiting_storage()
        {
            playfield[2, 1] = new PlayfieldSquare(SquareType.AmmoStorage, 0);
            playfield[3, 1] = new PlayfieldSquare(SquareType.CementStorage, 0);
            playfield[4, 1] = new PlayfieldSquare(SquareType.HamsterStorage, 0);
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);

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
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_cement_storage_player_should_replenish_cement()
        {
            playfield[3, 4] = new PlayfieldSquare(SquareType.CementStorage, 0);
            board = new BoardState(playfield, horizontalWalls, verticalWalls, holes,
                centaur, startingPositions);
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

            Assert.True(moveDescription.Contains("There is treasure here."));

            game.PerformMove(MoveType.DoNothing);

            Assert.True(player1.CarriesTreasure);
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
    }
}