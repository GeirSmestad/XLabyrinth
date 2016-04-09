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
                    if (board.GetWallAbovePlayfieldCoordinate(player.X, player.Y).IsPassable)
                    {
                        player.Y--;
                        descriptionOfCurrentMove.Append("Walked north.");
                    } else
                    {
                        descriptionOfCurrentMove.Append("Hit wall.");
                    }
                    
                    break;
                case MoveType.MoveDown:
                    if (board.GetWallBelowPlayfieldCoordinate(player.X, player.Y).IsPassable)
                    {
                        player.Y++;
                        descriptionOfCurrentMove.Append("Walked south.");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall.");
                    }
                    break;
                case MoveType.MoveLeft:
                    if (board.GetWallLeftOfPlayfieldCoordinate(player.X, player.Y).IsPassable)
                    {
                        player.X--;
                        descriptionOfCurrentMove.Append("Walked west.");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall.");
                    }
                    break;
                case MoveType.MoveRight:
                    if (board.GetWallRightOfPlayfieldCoordinate(player.X, player.Y).IsPassable)
                    {
                        player.X++;
                        descriptionOfCurrentMove.Append("Walked east.");
                    }
                    else
                    {
                        descriptionOfCurrentMove.Append("Hit wall.");
                    }
                    break;
                case MoveType.DoNothing:
                    descriptionOfCurrentMove.Append("Standing still.");
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

            switch (action)
            {
                case MoveType.BuildWallUp:
                    return;
                case MoveType.BuildWallRight:
                    return;
                case MoveType.BuildWallDown:
                    return;
                case MoveType.BuildWallLeft:
                    return;
            }
        }

        private void handleHamsterSpraying(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;

            switch (action)
            {
                case MoveType.HamsterSprayUp:
                    return;
                case MoveType.HamsterSprayRight:
                    return;
                case MoveType.HamsterSprayDown:
                    return;
                case MoveType.HamsterSprayLeft:
                    return;
            }
        }

        private void handleHamsterPlacement(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;

            switch (action)
            {
                case MoveType.PlaceHamsterUp:
                    return;
                case MoveType.PlaceHamsterRight:
                    return;
                case MoveType.PlaceHamsterDown:
                    return;
                case MoveType.PlaceHamsterLeft:
                    return;
            }
        }

        private void handleGrenadeThrow(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;

            switch (action)
            {
                case MoveType.ThrowGrenadeUp:
                    return;
                case MoveType.ThrowGrenadeRight:
                    return;
                case MoveType.ThrowGrenadeDown:
                    return;
                case MoveType.ThrowGrenadeLeft:
                    return;
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
