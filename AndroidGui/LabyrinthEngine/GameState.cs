using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.Moves;

namespace LabyrinthEngine
{
    public class GameState
    {
        BoardState Board;

        List<Player> Players;
        List<Action> CompletedMoves;

        int moveCounter;

        Random randomNumberGenerator;
        int initialRngSeed; // Must store this to facilitate rebuilding game state from move list

        public GameState(BoardState board)
        {
            Board = board;
        }

        public void SetToInitialState()
        {

        }

        /// <summary>
        /// A move consists of a movement action plus a followup action, which
        /// is any action that may modify the board: Fire, hamster, hamsterspray or
        /// cement.
        /// </summary>
        /// <returns>A description of the result of the move.</returns>
        public string PerformMoveForCurrentPlayer(MoveType movement, MoveType followup)
        {
            var currentPlayer = getCurrentPlayer();

            return "not implemented";

        }

        private Player getCurrentPlayer()
        {
            return Players[moveCounter % Players.Count];
        }
    }
}