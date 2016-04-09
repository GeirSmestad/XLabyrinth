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
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_hamster_storage_player_should_replenish_hamster_gear()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_fitness_studio_dead_player_should_see_it_and_be_resurrected()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_fitness_studio_live_player_should_see_empty_room()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_cement_storage_player_should_replenish_cement()
        {
            Assert.Fail("Not implemented");
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
        public void When_constructing_wall_wall_should_be_created()
        {
            Assert.Fail("Not implemented");

            // Test that cement has been used
        }

        [Test]
        public void When_attempting_to_construct_wall_on_wall_should_yield_no_result()
        {
            Assert.Fail("Not implemented");

            // Test that no cement has been used
        }

        [Test]
        public void When_blowing_up_wall_wall_should_disappear()
        {
            buildWallsAroundSquare(1, 1);
            player1.X = 1;
            player1.Y = 1;
            player1.NumGrenades = 4;
            var expectedMessage = "You blow up the wall.";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallAbovePlayfieldCoordinate(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallRightOfPlayfieldCoordinate(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallBelowPlayfieldCoordinate(1, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsTrue(board.GetWallLeftOfPlayfieldCoordinate(1, 1).IsPassable);

            Assert.AreEqual(player1.NumGrenades, 0, "When using grenades, grenade count should decrease");
        }

        [Test]
        public void When_blowing_up_exterior_wall_should_see_message_and_no_result()
        {
            Assert.Fail("Not implemented");

        }

        [Test]
        public void When_blowing_up_hamster_wall_should_see_message_and_no_result()
        {
            buildHamsteredWallsAroundSquare(1, 1);
            player1.X = 3;
            player1.Y = 1;
            player1.NumGrenades = 4;
            var expectedMessage = "A hamster returns your grenade with the pin inserted.";

            game.PerformMove(MoveType.DoNothing);
            var message = game.PerformMove(MoveType.ThrowGrenadeUp);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallAbovePlayfieldCoordinate(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeRight);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallRightOfPlayfieldCoordinate(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeDown);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallBelowPlayfieldCoordinate(3, 1).IsPassable);

            game.PerformMove(MoveType.DoNothing);
            message = game.PerformMove(MoveType.ThrowGrenadeLeft);

            Assert.AreEqual(message, expectedMessage);
            Assert.IsFalse(board.GetWallLeftOfPlayfieldCoordinate(3, 1).IsPassable);

            Assert.AreEqual(player1.NumGrenades, 4, "Hamster walls should not spend grenades");
        }

        [Test]
        public void When_visiting_treasure_player_should_take_it()
        {
            player1.X = 3;
            player1.Y = 3;
            board.GetPlayfieldSquareAt(3, 2).NumTreasures = 2;

            game.PerformMove(MoveType.MoveUp);
            game.PerformMove(MoveType.DoNothing);

            Assert.True(player1.CarriesTreasure);
            Assert.AreEqual(board.GetPlayfieldSquareAt(3, 2).NumTreasures, 1);
        }

        [Test]
        public void When_visiting_treasure_when_already_carrying_player_should_see_and_leave_it()
        {
            player1.X = 3;
            player1.Y = 3;
            player1.CarriesTreasure = true;
            board.GetPlayfieldSquareAt(3, 2).NumTreasures = 2;

            string moveDescription = game.PerformMove(MoveType.MoveUp);

            Assert.True(moveDescription.Contains("There is treasure here."));

            game.PerformMove(MoveType.DoNothing);

            Assert.True(player1.CarriesTreasure);
            Assert.AreEqual(board.GetPlayfieldSquareAt(3, 2).NumTreasures, 2);
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
            board.GetWallAbovePlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallRightOfPlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallBelowPlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallLeftOfPlayfieldCoordinate(x, y).IsPassable = false;
        }

        private void buildHamsteredWallsAroundSquare(int x, int y)
        {
            board.GetWallAbovePlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallRightOfPlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallBelowPlayfieldCoordinate(x, y).IsPassable = false;
            board.GetWallLeftOfPlayfieldCoordinate(x, y).IsPassable = false;

            board.GetWallAbovePlayfieldCoordinate(x, y).HasHamster = true;
            board.GetWallRightOfPlayfieldCoordinate(x, y).HasHamster = true;
            board.GetWallBelowPlayfieldCoordinate(x, y).HasHamster = true;
            board.GetWallLeftOfPlayfieldCoordinate(x, y).HasHamster = true;
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