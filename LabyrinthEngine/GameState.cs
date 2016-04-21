using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Moves;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.GameLogic;

namespace LabyrinthEngine
{
    public class GameState
    {
        public BoardState Board { get; private set; }

        public List<Player> Players { get; private set; }
        public int MoveCounter { get; internal set; }
        public TurnPhase CurrentTurnPhase { get; internal set; }

        private List<Move> completedMoves; // All moves in the current game, stored for save & undo
        private Random randomNumberGenerator;
        private int initialRngSeed; // Must store this to facilitate rebuilding game state from completedMoves
        private BoardState initialBoardState;
        private List<Player> initialPlayersState;
        private int currentUndoStep;

        private TurnController turnController;

        public GameState(BoardState board, List<Player> players)
        {
            Board = board;
            Players = players;

            initialRngSeed = DateTime.Now.GetHashCode();
            initialBoardState = HelperMethods.DeepClone(Board);
            initialPlayersState = HelperMethods.DeepClone(Players);
            completedMoves = new List<Move>();

            setGameToInitialState();
        }

        public GameState(BoardState board, List<Player> players, int initialRngSeed)
        {
            Board = board;
            Players = players;

            this.initialRngSeed = initialRngSeed;
            initialBoardState = HelperMethods.DeepClone(Board);
            initialPlayersState = HelperMethods.DeepClone(Players);
            completedMoves = new List<Move>();

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

            /* TODO: Resetting the board from initial state breaks most tests since it removes
             * the link to the objects that the tests are initialized from. Have to rewrite
             * the tests in a clever way before this (and hence undo) can be implemented. */
            //Board = HelperMethods.DeepClone(initialBoardState);
            // TODO: Also need to reset player state for undo/redo to work.

            setPlayersToInitialPositions();
            turnController = new TurnController(Board, Players, randomNumberGenerator);
            currentUndoStep = completedMoves.Count;
        }

        public Player CurrentPlayer()
        {
            // Moves are always executed & stored in pairs, with each player performing two moves -
            // one main action and one followup action. This implementation detail is hidden from
            // the public API.
            if (MoveCounter % 2 == 0)
            {
                return Players[MoveCounter/2 % Players.Count];
            }
            else
            {
                return Players[(MoveCounter-1)/2 % Players.Count];
            }
        }

        /// <summary>
        /// Performs the specified move for the current player. If an illegal action is specified
        /// (e.g. movement during the followup phase), "DoNothing" is performed and the game proceeds.
        /// </summary>
        /// <returns>A description of what happened during and after the move.</returns>
        public string PerformMove(MoveType action)
        {
            removeRedoHistory();

            if (CurrentTurnPhase == TurnPhase.SelectMainAction)
            {
                performMainActionForCurrentPlayer(action);
                return turnController.DescriptionOfExecutedMove();
            }
            else
            {
                performFollowupActionForCurrentPlayer(action);
                return turnController.DescriptionOfExecutedMove();
            }
        }

        /// <summary>
        /// Performs the first action for the current player. For convenience, if this method
        /// is called with a non-movement action, it will execute a "DoNothing" action and then
        /// execute the action as a followup action. This is equivalent to the player skipping
        /// his movement and then executing the action as a followup.
        /// </summary>
        /// <returns>A description of the result of the move.</returns>
        private void performMainActionForCurrentPlayer(MoveType action)
        {
            // TODO: This could also move into TurnController to better encapsulate functionality
            var player = CurrentPlayer();
            var move = new Move(player, action);

            if (TurnController.IsMovementAction(action))
            {
                turnController.ResolveMovementAction(move);
                turnController.ResolvePostMovementEventsFor(player);
                updateTurnStateBasedOn(move);
            }
            else
            {
                updateTurnStateBasedOn(new Move(player,MoveType.DoNothing));
                performFollowupActionForCurrentPlayer(action);
            }
        }

        /// <returns>A description of the result of the move.</returns>
        private void performFollowupActionForCurrentPlayer(MoveType action)
        {
            // TODO: This could also move into TurnController to better encapsulate functionality
            var player = CurrentPlayer();
            var move = new Move(player, action);
            turnController.ResolveFollowupAction(move);

            if (CurrentPlayer() == Players[Players.Count - 1])
            {
                turnController.ResolveEndOfTurnEvents();
            }

            updateTurnStateBasedOn(move);
        }

        private void updateTurnStateBasedOn(Move previousMove)
        {
            // TODO: This could also move into TurnController to better encapsulate functionality
            MoveCounter++;
            if (CurrentTurnPhase == TurnPhase.SelectMainAction)
            {
                CurrentTurnPhase = TurnPhase.SelectFollowupAction;
            }
            else
            {
                CurrentTurnPhase = TurnPhase.SelectMainAction;
            }

            if (currentUndoStep == 0)
            {
                // If no undo operation in progress, add move to list.
                completedMoves.Add(previousMove);
            }
        }

        public void UndoPreviousMove()
        {
            if (CanUndo())
            {
                currentUndoStep++;
                stepToBeforeMoveNumber(completedMoves.Count - currentUndoStep);
            }
        }

        public void RedoNextMove()
        {
            if (CanRedo())
            {
                stepToBeforeMoveNumber(completedMoves.Count - currentUndoStep-1);
            }
            currentUndoStep--;
        }

        public bool CanUndo()
        {
            return currentUndoStep < completedMoves.Count;
        }

        public bool CanRedo()
        {
            return currentUndoStep > 0;
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

        private void stepToBeforeMoveNumber(int n)
        {
            setGameToInitialState();

            for (int i = 0; i < n; i++)
            {
                var nextMoveToExecute = completedMoves[i].ActionType;
                performMainActionForCurrentPlayer(nextMoveToExecute); // Also performs followup actions
            }
        }

        private void setPlayersToInitialPositions()
        {
            var startingPositions = Board.StartingPositions;

            if (startingPositions.Count >= Players.Count)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    Players[i].X = startingPositions[i].X;
                    Players[i].Y = startingPositions[i].Y;
                }
            }
            else
            {
                for (int i = 0; i < startingPositions.Count; i++)
                {
                    Players[i].X = startingPositions[i].X;
                    Players[i].Y = startingPositions[i].Y;
                }

                for (int j = startingPositions.Count; j < Players.Count; j++)
                {
                    Players[j].X = randomNumberGenerator.Next(0, Board.Width - 1);
                    Players[j].Y = randomNumberGenerator.Next(0, Board.Width - 1);
                }
            }
        }
    }
}