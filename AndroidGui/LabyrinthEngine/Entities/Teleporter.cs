using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Entities
{
    /// <summary>
    /// A hole, in Labyrinth terminology, is a teleporter. When you fall through a hole,
    /// you end up on top of the next hole awaiting your next turn. The holes are connected 
    /// in a loop.
    /// </summary>
    [Serializable]
    public class Teleporter
    {
        public int TeleporterIndex { get; private set; }
        public Teleporter NextHole { get; private set; }
        //public PlayfieldSquare Square { get; private set; }
        
        public Teleporter(int teleporterIndex, Teleporter nextHole)
        {
            TeleporterIndex = teleporterIndex;
            NextHole = nextHole;
            //Square = square;
        }
    }
}