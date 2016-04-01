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

namespace AndroidGui.Core.Moves
{
    class Move
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