using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndroidGui.Core.Entities;

namespace AndroidGui.Core.Playfield
{
    /// <summary>
    /// An instance of the Board class represents the current state of the
    /// play area.
    /// </summary>
    public class BoardState
    {
        /* The playfield is an (x,y)-grid of positive numbers with (0,0) in the top-left.
         
        Horizontal and vertical walls are defined separately. 
        
        Each horizontal wall has the coordinates (w_y, x).
            w_y is the y coordinate of the wall, with 0 being the top wall
            x is the (unambiguous) x coordinate of the playfield squares that the wall borders on

        Each vertical wall has the coordinates (w_x, y).
            w_x is the x coordinate of the wall, with 0 being the leftmost wall
            y is the (unambiguous) y coordinate of the playfield squares that the wall borders on

        This grid system was chosen because it lets us abstract away the wall
        coordinate logic, so we don't have to think about it in the game logic. */

        private WallSection[,] HorizontalWalls;
        private WallSection[,] VerticalWalls;
        private PlayfieldSquare[,] PlayfieldGrid;
        private List<Teleporter> Holes; // TODO: Unsure if this needs to be here; might be double bookkeeping
        private Centaur centaur;

        public BoardState(PlayfieldSquare[,] playfieldGrid, 
            WallSection[,] horizontalWalls, WallSection[,] verticalWalls,
            List<Teleporter> holes, Centaur centaur)
        {
            HorizontalWalls = horizontalWalls;
            VerticalWalls = verticalWalls;
            PlayfieldGrid = playfieldGrid;
            Holes = holes;
            this.centaur = centaur;
        }

        public PlayfieldSquare GetPlayfieldSquareAt(int x, int y)
        {
            return PlayfieldGrid[x, y];
        }

        public WallSection GetWallAbovePlayfieldCoordinate(int x, int y)
        {
            throw new NotImplementedException();
        }

        public WallSection GetWallBelowPlayfieldCoordinate(int x, int y)
        {
            throw new NotImplementedException();
        }

        public WallSection GetWallLeftOfPlayfieldCoordinate(int x, int y)
        {
            throw new NotImplementedException();
        }

        public WallSection GetWallRightOfPlayfieldCoordinate(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}