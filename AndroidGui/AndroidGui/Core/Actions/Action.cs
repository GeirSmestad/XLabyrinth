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
using AndroidGui.Core.Actors;

namespace AndroidGui.Core
{
    class Action
    {
        public Player PerformedBy { get; private set; }
        public ActionType ActionType { get; private set; }

        public Action(Player performedBy, ActionType actionType)
        {
            PerformedBy = performedBy;
            ActionType = actionType;
        }
    }
}