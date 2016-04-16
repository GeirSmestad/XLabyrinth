using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;

namespace LabyrinthEngine.Moves
{
    public class Move
    {
        public Player PerformedBy { get; private set; }
        public MoveType ActionType { get; private set; }

        public Move(Player performedBy, MoveType actionType)
        {
            PerformedBy = performedBy;
            ActionType = actionType;
        }
    }
}