using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Entities
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
        public int Score = 0;

        public bool IsAlive = true;
        public int PosX;
        public int PosY;

        public void Die()
        {
            IsAlive = false;
            NumArrows = NumGrenades = NumHamsters = NumHamsterSprays = 0;
        }
    }
}