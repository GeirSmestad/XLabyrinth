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

        /// <returns>A description of the result of the move.</returns>
        public string PerformMainActionForCurrentPlayer(MoveType action)
        {
            var player = CurrentPlayer();

            // If main action is not movement, go to next player

            teleportIfStandsOnTeleporter(player);
            removeRedoHistory();
            updateTurnStateBasedOn(action);
            
            return "not implemented";
        }

        /// <returns>A description of the result of the move.</returns>
        public string PerformFollowupActionForCurrentPlayer(MoveType action)
        {
            var player = CurrentPlayer();

            teleportIfStandsOnTeleporter(player);
            removeRedoHistory();
            updateTurnStateBasedOn(action);
            return "not implemented";
        }

        private void updateTurnStateBasedOn(MoveType previousAction)
        {
            MoveCounter++;

            completedMoves.Add(new Move(CurrentPlayer(), previousAction));
            // TODO: Update TurnPhase and current player based on action & current TurnPhase
            // TODO: Might replace this with a "resolveTurn" method, which handles more
        }

        private void teleportIfStandsOnTeleporter(Player player)
        {

        }

        private void moveCentaur()
        {

        }

        public void UndoPreviousMove()
        {
            // Remember to update TurnPhase
        }

        public void RedoNextMove()
        {
            // Remember to update TurnPhase
        }

        private void stepToMoveNumber(int n)
        {
            setGameToInitialState();
            // Reset to initial state and replay all moves up to move n in completedMoves.
        }

        private void removeRedoHistory()
        {
            if (currentUndoStep > 0)
            {
                // Remove last "currentUndoStep" entries from completed moves.
                completedMoves.RemoveRange(completedMoves.Count-currentUndoStep, currentUndoStep);
                currentUndoStep = 0;
            }
        }
    }
}