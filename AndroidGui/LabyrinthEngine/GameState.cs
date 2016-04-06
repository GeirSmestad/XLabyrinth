using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Helpers;

namespace LabyrinthEngine
{
    public class GameState
    {
        public BoardState Board { get; private set; }

        public List<Player> Players { get; private set; }
        public int MoveCounter { get; private set; } // TODO: Might be redundant copy of completedMoves.Count
        public TurnPhase CurrentTurnPhase { get; private set; }

        private List<Move> completedMoves;
        private Random randomNumberGenerator;
        private int initialRngSeed; // Must store this to facilitate rebuilding game state from move list
        private BoardState initialBoardState;
        private int currentUndoStep;

        public GameState(BoardState board)
        {
            Board = board;

            initialRngSeed = DateTime.Now.GetHashCode();
            initialBoardState = HelperMethods.DeepClone(Board);

            setGameToInitialState();
        }

        /// <summary>
        /// Sets the game to its initial state, but keep the list of completed moves, if any.
        /// In effect, this performs an undo back to the very first move of the game.
        /// </summary>
        private void setGameToInitialState()
        {
            randomNumberGenerator = new Random(initialRngSeed);
            MoveCounter = 0;
            CurrentTurnPhase = TurnPhase.SelectMainAction;
            Board = HelperMethods.DeepClone(initialBoardState);
            currentUndoStep = completedMoves.Count;
        }

        public Player CurrentPlayer()
        {
            return Players[MoveCounter % Players.Count];
        }

        /// <summary>
        /// Performs the movement action for the current player. For convenience, if this method
        /// is called with a non-movement action, it will execute a "DoNothing" action and then
        /// execute the action as a followup action. This is equivalent to the player skipping
        /// his movement and then executing the action as a followup.
        /// </summary>
        /// <returns>A description of the result of the move.</returns>
        public string PerformMovementActionForCurrentPlayer(MoveType action)
        {
            var player = CurrentPlayer();
            var move = new Move(player, action);
            removeRedoHistory();

            if (IsMovementAction(action))
            {
                var actionResult = resolveMovementAndReturnResult(move);
                actionResult += resolvePostMovementEventsFor(player);
                updateTurnStateBasedOn(move);

                return actionResult;
            }
            else
            {
                updateTurnStateBasedOn(new Move(player,MoveType.DoNothing));
                return PerformFollowupActionForCurrentPlayer(action);
            }
        }

        /// <returns>A description of the result of the move.</returns>
        public string PerformFollowupActionForCurrentPlayer(MoveType action)
        {
            var player = CurrentPlayer();
            var move = new Move(player, action);
            removeRedoHistory();
            
            updateTurnStateBasedOn(move);
            return "not implemented";
        }

        // TODO: Extract to helper class? These methods could be moved to e.g. an
        // "ActionPerformer" class which executes the desired actions and returns the
        // result as a string.
        private string resolveMovementAndReturnResult(Move move)
        {
            var player = move.PerformedBy;
            var action = move.ActionType;
            string actionResultDescription;

            switch (action)
            {
                case MoveType.MoveUp:
                    actionResultDescription = "not implemented";
                    break;
                case MoveType.MoveDown:
                    actionResultDescription = "not implemented";
                    break;
                case MoveType.MoveLeft:
                    actionResultDescription = "not implemented";
                    break;
                case MoveType.MoveRight:
                    actionResultDescription = "not implemented";
                    break;
                case MoveType.DoNothing:
                    actionResultDescription = player.Name + "waits.";
                    break;
                case MoveType.FallThroughHole:
                    //teleportIfStandsOnTeleporter(player);
                    actionResultDescription = "Fall through teleporter not implemented";
                    break;
                default:
                    throw new InvalidOperationException("Fall-through in game logic: "
                        + "Action should have been executed as a followup action.");
            }

            return actionResultDescription;
        }

        // TODO: Extract to helper class?
        private string resolvePostMovementEventsFor(Player player)
        {
            return "not implemented";
        }

        private string resolveFollowupAndReturnResult(Move move)
        {
            return "not implemented";
        }

        private void updateTurnStateBasedOn(Move previousMove)
        {
            // TODO: This breaks redo functionality. Must be able to execute moves
            // without modifying the completedMoves list or clearing redo.

            MoveCounter++;
            completedMoves.Add(previousMove);
            if (CurrentTurnPhase == TurnPhase.SelectMainAction)
            {
                CurrentTurnPhase = TurnPhase.SelectFollowupAction;
            }
            else
            {
                CurrentTurnPhase = TurnPhase.SelectMainAction;
            }
        }

        private void teleportIfStandsOnTeleporter(Player player)
        {

        }

        private void moveCentaur()
        {

        }

        public void UndoPreviousMove()
        {
            if (currentUndoStep < completedMoves.Count)
            {
                currentUndoStep++;
                stepToBeforeMoveNumber(completedMoves.Count - currentUndoStep);
            }
        }

        public void RedoNextMove()
        {
            // Remember to update TurnPhase
        }

        private void stepToBeforeMoveNumber(int n)
        {
            setGameToInitialState();

            for (int i = 0; i < n; i++)
            {
                var nextMoveToExecute = completedMoves[i].ActionType;
                PerformMovementActionForCurrentPlayer(nextMoveToExecute); // Also performs followup actions
            }
        }

        private void removeRedoHistory()
        {
            if (currentUndoStep > 0)
            {
                // Remove last "currentUndoStep" entries from completed moves.
                completedMoves.RemoveRange(completedMoves.Count - currentUndoStep, currentUndoStep);
                currentUndoStep = 0;
            }
        }

        private bool IsMovementAction(MoveType action)
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