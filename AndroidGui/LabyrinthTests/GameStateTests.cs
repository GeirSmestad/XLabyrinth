using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using LabyrinthEngine;
using LabyrinthEngine.Playfield;

namespace AndroidGui.Tests
{
    [TestFixture]
    public class GameStateTests
    {
        BoardState board; 
        GameState game;

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void Should_pass()
        {
            Assert.IsTrue(true);
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
            Assert.Fail("Not implemented");

            // Test moving UDLR in walled-off roomand assert position is identical at each step
        }

        [Test]
        public void When_moving_in_open_area_player_should_move()
        {
            Assert.Fail("Not implemented");
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
            Assert.Fail("Not implemented");

            // Test that a bomb has been used
        }

        [Test]
        public void When_blowing_up_exterior_wall_should_see_message_and_no_result()
        {
            Assert.Fail("Not implemented");

            // Test that a bomb has been used
        }

        [Test]
        public void When_blowing_up_hamster_wall_should_see_message_and_no_result()
        {
            Assert.Fail("Not implemented");

            // Test that a bomb has not been used
        }

        [Test]
        public void When_visiting_treasure_player_should_take_it()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void When_visiting_treasure_when_already_carrying_player_should_leave_it()
        {
            Assert.Fail("Not implemented");
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

        // Game management tests

        [Test]
        public void Saving_and_loading_game_state_should_work()
        {
            Assert.Fail("Not implemented");
        }

        [Test]
        public void Undo_and_redo_moves_should_work()
        {
            Assert.Fail("Not implemented");
        }
    }
}