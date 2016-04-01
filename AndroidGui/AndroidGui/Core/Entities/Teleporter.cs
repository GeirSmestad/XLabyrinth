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

namespace AndroidGui.Core.Entities
{
    /// <summary>
    /// A hole, in Labyrinth terminology, is a teleporter. When you fall through a hole,
    /// you end up on top of the next hole awaiting your next turn. The holes are connected 
    /// in a loop.
    /// </summary>
    class Teleporter
    {
        public Teleporter(Teleporter nextHole)
        {
            NextHole = nextHole;
        }

        public Teleporter NextHole { get; private set; }
    }
}