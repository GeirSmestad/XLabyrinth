using LabyrinthEngine.GameLogic;
using LabyrinthEngine.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Entities
{
    [Serializable]
    public class Player
    {
        public string Name;

        public const int ArrowCapacity = 2;
        public const int GrenadeCapacity = 2;
        public const int HamsterCapacity = 2;
        public const int HamsterSprayCapacity = 2;
        public const int CementCapacity = 2;

        public int NumArrows = 2;
        public int NumGrenades = 2;
        public int NumHamsters = 0;
        public int NumHamsterSprays = 0;
        public int NumCement = 0;
        public bool CarriesTreasure = false;
        public int Score = 0;

        public bool IsAlive = true;
        public int X;
        public int Y;
        public Position PositionBeforePreviousMovementAction;
        public OutsideLabyrinthPhase OutsideLabyrinthPhase;

        public bool IsOutsideLabyrinth()
        {
            return OutsideLabyrinthPhase != OutsideLabyrinthPhase.Inside;
        }
    }
}