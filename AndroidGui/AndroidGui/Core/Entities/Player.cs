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
    class Player
    {
        public string Name;

        const int arrowCapacity = 2;
        const int grenadeCapacity = 2;
        const int hamsterCapacity = 2;
        const int hamsterSprayCapacity = 2;
        const int cementCapacity = 2;

        public int NumArrows = 2;
        public int NumGrenades = 2;
        public int NumHamsters = 0;
        public int NumHamsterSprays = 0;
        public int NumCement = 0;
        public bool CarriesTreasure = false;

        public bool IsGhost = false;
        public int PosX;
        public int PosY;


        public void Die()
        {
            IsGhost = true;
            NumArrows = NumGrenades = NumHamsters = NumHamsterSprays = 0;
        }
    }
}