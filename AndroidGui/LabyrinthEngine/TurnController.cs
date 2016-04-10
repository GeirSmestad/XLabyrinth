using LabyrinthEngine.Entities;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine
{
    /// <summary>
    /// Performs internal actions to update the game state during the course of the game.
    /// Most of the game rules are described in this class. The class belongs to and is 
    /// controlled by an instance of the GameState class.
    /// </summary>
    public class TurnController
    {
        private BoardState board;
        private List<Player> players;

        private StringBuilder descriptionOfCurrentMove;

        public TurnController(GameState game)
        {
            board = game.Board;
            players = game.Players;
        }

        public string DescriptionOfExecutedMove()
        {
            return descriptionOfCurrentMove.ToString();
        }

        public void ResolveMovementAction(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            descriptionOfCurrentMove = new StringBuilder();

            switch (action)
            {
                case MoveType.MoveUp:
                    if (board.GetWallAbove(player).IsPassable)
                    {
                        player.Y--;
                        descriptionOfCurrentMove.Append("Walked north. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    
                    break;
                case MoveType.MoveDown:
                    if (board.GetWallBelow(player).IsPassable)
                    {
                        player.Y++;
                        descriptionOfCurrentMove.Append("Walked south. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.MoveLeft:
                    if (board.GetWallLeftOf(player).IsPassable)
                    {
                        player.X--;
                        descriptionOfCurrentMove.Append("Walked west. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.MoveRight:
                    if (board.GetWallRightOf(player).IsPassable)
                    {
                        player.X++;
                        descriptionOfCurrentMove.Append("Walked east. ");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall. ");
                    }
                    break;
                case MoveType.DoNothing:
                    descriptionOfCurrentMove.Append("Standing still. ");
                    break;
                case MoveType.FallThroughHole:
                    //teleportIfStandsOnTeleporter(player);
                    descriptionOfCurrentMove.Append("Fall through teleporter not implemented");
                    break;
                default:
                    throw new InvalidOperationException("Fall-through in game logic: "
                        + "Action should have been executed as a followup action.");
            }
        }

        public void ResolveFollowupAction(Move move)
        {
            var action = move.ActionType;
            descriptionOfCurrentMove = new StringBuilder();

            if (action == MoveType.FireUp ||
                action == MoveType.FireRight ||
                action == MoveType.FireDown ||
                action == MoveType.FireLeft)
            {
                handleShot(move);
            }
            else if (action == MoveType.ThrowGrenadeUp ||
                     action == MoveType.ThrowGrenadeRight ||
                     action == MoveType.ThrowGrenadeDown ||
                     action == MoveType.ThrowGrenadeLeft)
            {
                handleGrenadeThrow(move);
            }
            else if (action == MoveType.PlaceHamsterUp ||
                     action == MoveType.PlaceHamsterRight ||
                     action == MoveType.PlaceHamsterDown ||
                     action == MoveType.PlaceHamsterLeft)
            {
                handleHamsterPlacement(move);
            }
            else if (action == MoveType.HamsterSprayUp ||
                     action == MoveType.HamsterSprayRight ||
                     action == MoveType.HamsterSprayDown ||
                     action == MoveType.HamsterSprayLeft)
            {
                handleHamsterSpraying(move);
            }
            else if (action == MoveType.BuildWallUp ||
                     action == MoveType.BuildWallRight ||
                     action == MoveType.BuildWallDown ||
                     action == MoveType.BuildWallLeft)
            {
                handleWallConstruction(move);
            }
            else if (action == MoveType.DoNothing || 
                     action == MoveType.FallThroughHole ||
                     action == MoveType.MoveUp ||
                     action == MoveType.MoveRight ||
                     action == MoveType.MoveDown ||
                     action == MoveType.MoveLeft)
            {
                return; // Skip invalid followup actions.
            }
            else
            {
                throw new InvalidOperationException("Fall-through in game logic: "
                + "Unhandled followup action.");
            }
        }

        private void handleWallConstruction(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.BuildWallUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.BuildWallRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.BuildWallDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.BuildWallLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleWallConstruction called with illegal move");
            }

            if (player.NumCement <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of cement. ");
            }
            else if (targetWall.IsPassable)
            {
                player.NumCement--;
                targetWall.IsPassable = false;
                descriptionOfCurrentMove.Append("You construct a new wall. ");
            }
            else
            {
                descriptionOfCurrentMove.Append("There is already a wall there. ");
            }
        }

        private void handleHamsterSpraying(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.HamsterSprayUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.HamsterSprayRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.HamsterSprayDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.HamsterSprayLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleHamsterSpraying called with illegal move");
            }

            if (player.NumHamsterSprays <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of cement. ");
            }
            else if (targetWall.HasHamster)
            {
                player.NumHamsterSprays--;
                targetWall.HasHamster = false;
                descriptionOfCurrentMove.Append("Several squeaking hamsters tumble out from the wall. ");
            }
            else
            {
                player.NumHamsterSprays--;
                descriptionOfCurrentMove.Append("You spray the wall. ");
            }
        }

        private void handleHamsterPlacement(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.PlaceHamsterUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.PlaceHamsterRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.PlaceHamsterDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.PlaceHamsterLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleHamsterPlacement called with illegal move");
            }

            if (player.NumHamsters <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of hamsters. ");
            }
            else if (!targetWall.HasHamster)
            {
                player.NumHamsters--;
                targetWall.HasHamster = true;
                descriptionOfCurrentMove.Append("Several squeaking hamsters tumble out from the wall. ");
            }
            else
            {
                descriptionOfCurrentMove.Append("A family of hamsters angrily repels your rodent. ");
            }
        }

        private void handleGrenadeThrow(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            WallSection targetWall;

            switch (action)
            {
                case MoveType.ThrowGrenadeUp:
                    targetWall = board.GetWallAbove(player);
                    break;
                case MoveType.ThrowGrenadeRight:
                    targetWall = board.GetWallRightOf(player);
                    break;
                case MoveType.ThrowGrenadeDown:
                    targetWall = board.GetWallBelow(player);
                    break;
                case MoveType.ThrowGrenadeLeft:
                    targetWall = board.GetWallLeftOf(player);
                    break;
                default:
                    throw new InvalidOperationException("handleGrenadeThrow called with illegal move");
            }

            if (player.NumGrenades <= 0)
            {
                descriptionOfCurrentMove.Append("You are out of grenades.");
            }
            else if (targetWall.IsExterior)
            {
                player.NumGrenades--;
                descriptionOfCurrentMove.Append("The grenade explodes against an exterior wall. ");
            }
            else if (targetWall.HasHamster)
            {
                descriptionOfCurrentMove.Append("A hamster returns your grenade with the pin inserted. ");
            }
            else if (targetWall.IsPassable)
            {
                player.NumGrenades--;
                descriptionOfCurrentMove.Append("");
            }
            else
            {
                player.NumGrenades--;
                targetWall.IsPassable = true;
                descriptionOfCurrentMove.Append("You blow up the wall. ");
            }
        }

        private void handleShot(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;

            switch (action)
            {
                case MoveType.FireUp:
                    return;
                case MoveType.FireRight:
                    return;
                case MoveType.FireDown:
                    return;
                case MoveType.FireLeft:
                    return;
            }
        }

        public void ResolvePostMovementEventsFor(Player player)
        {
            // pick up treasures
            // see treasures if not picking up
            // fall through teleporters
            // die if visiting centaur
            // print clopclop if centaur nearby
        }

        private void ResolveEndOfTurn()
        {
            // Move centaur if end of turn

        }

        private void teleportIfStandsOnTeleporter(Player player)
        {

        }

        private void moveCentaur()
        {

        }

        public static bool IsMovementAction(MoveType action)
        {
            return action == MoveType.DoNothing ||
                action == MoveType.FallThroughHole ||
                action == MoveType.MoveDown ||
                action == MoveType.MoveLeft ||
                action == MoveType.MoveRight ||
                action == MoveType.MoveUp;
        }
    }
}
