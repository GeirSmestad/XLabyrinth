using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;

namespace LabyrinthEngine.Playfield
{
    /// <summary>
    /// An instance of the Board class represents the current state of the
    /// play area.
    /// </summary>
    ///
    [Serializable]
    public class BoardState
    {
        /* The playfield is an (x,y)-grid of positive numbers with (0,0) in the top-left.
         
        Horizontal and vertical walls are defined separately. 
        
        Each horizontal wall has the coordinates (x, w_y).
            x is the (unambiguous) x coordinate of the playfield squares that the wall borders on
            w_y is the y coordinate of the wall, with 0 being the top wall

        Each vertical wall has the coordinates (y, w_x).
            y is the (unambiguous) y coordinate of the playfield squares that the wall borders on
            w_x is the x coordinate of the wall, with 0 being the leftmost wall
            
        This grid system was chosen because it lets us abstract away the wall
        coordinate logic, so we don't have to think about it in the game logic. */

        public WallSection[,] HorizontalWalls { get; private set; }
        public WallSection[,] VerticalWalls { get; private set; }
        public PlayfieldSquare[,] PlayfieldGrid { get; private set; }
        public List<Teleporter> Holes { get; private set; }
        public Centaur centaur { get; private set; }
        public List<Position> StartingPositions { get; private set; }

        public BoardState(PlayfieldSquare[,] playfieldGrid, 
            WallSection[,] horizontalWalls, WallSection[,] verticalWalls,
            List<Teleporter> holes, Centaur centaur, 
            List<Position> startingPositions)
        {
            HorizontalWalls = horizontalWalls;
            VerticalWalls = verticalWalls;
            PlayfieldGrid = playfieldGrid;
            Holes = holes;
            this.centaur = centaur;
            StartingPositions = startingPositions;
        }

        public PlayfieldSquare GetPlayfieldSquareAt(int x, int y)
        {
            return PlayfieldGrid[x, y];
        }

        public WallSection GetWallAbovePlayfieldCoordinate(int x, int y)
        {
            return HorizontalWalls[x, y];
        }

        public WallSection GetWallBelowPlayfieldCoordinate(int x, int y)
        {
            return HorizontalWalls[x, y+1];
        }

        public WallSection GetWallLeftOfPlayfieldCoordinate(int x, int y)
        {
            return VerticalWalls[y, x];
        }

        public WallSection GetWallRightOfPlayfieldCoordinate(int x, int y)
        {
            return VerticalWalls[y, x+1];
        }
    }
}