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
                        player.Y++;
                        descriptionOfCurrentMove.Append("Walked north.");
                    } else
                    {
                        descriptionOfCurrentMove.Append("Hit wall.");
                    }
                    
                    break;
                case MoveType.MoveDown:
                    if (board.GetWallBelowPlayfieldCoordinate(player.X, player.Y).IsPassable)
                    {
                        player.Y--;
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
            descriptionOfCurrentMove = new StringBuilder();
            //return "not implemented";
        }

        public void ResolvePostMovementEventsFor(Player player)
        {
            //return "not implemented";
        }

        private void ResolveEndOfTurn()
        {
            // Move centaur
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
