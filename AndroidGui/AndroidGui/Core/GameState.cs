using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidGui.Core.Entities;
using AndroidGui.Core.Playfield;
using AndroidGui.Core.Moves;

namespace AndroidGui.Core
{
    class GameState
    {
        Board BoardState;

        List<Player> Players;
        List<Action> CompletedMoves;

        int moveCounter;

        Random randomNumberGenerator;
        int initialRngSeed; // Must store this to facilitate rebuilding game state from move list

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