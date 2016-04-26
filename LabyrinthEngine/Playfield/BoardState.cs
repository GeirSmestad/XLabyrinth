using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Entities;
using LabyrinthEngine.Geometry;
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

            if (centaur == null)
            {
                throw new InvalidOperationException("The centaur cannot be null. If you want " +
                    "to play without the centaur, set its starting position to e.g. (-2,-2).");
            }
        }

        public int Width
        {
            get { return PlayfieldGrid.GetLength(0); }
        }

        public int Height
        {
            get { return PlayfieldGrid.GetLength(1); }
        }

        public PlayfieldSquare GetPlayfieldSquareOf(int playfieldX, int playfieldY)
        {
            return PlayfieldGrid[playfieldX, playfieldY];
        }

        public PlayfieldSquare GetPlayfieldSquareOf(Player player)
        {
            return GetPlayfieldSquareOf(player.X, player.Y);
        }

        public WallSection GetWallAbove(int playfieldX, int playfieldY)
        {
            var coordinate = HelperMethods.GetCoordinateOfWallAbove(playfieldX, playfieldY);
            return HorizontalWalls[coordinate.X, coordinate.W_y];
        }

        public WallSection GetWallAbove(Player player)
        {
            return GetWallAbove(player.X, player.Y);
        }

        public WallSection GetWallBelow(int playfieldX, int playfieldY)
        {
            var coordinate = HelperMethods.GetCoordinateOfWallBelow(playfieldX, playfieldY);
            return HorizontalWalls[coordinate.X, coordinate.W_y];
        }

        public WallSection GetWallBelow(Player player)
        {
            return GetWallBelow(player.X, player.Y);
        }

        public WallSection GetWallLeftOf(int playfieldX, int playfieldY)
        {
            var coordinate = HelperMethods.GetCoordinateOfWallLeftOf(playfieldX, playfieldY);
            return VerticalWalls[coordinate.Y, coordinate.W_x];
        }

        public WallSection GetWallLeftOf(Player player)
        {
            return GetWallLeftOf(player.X, player.Y);
        }

        public WallSection GetWallRightOf(int playfieldX, int playfieldY)
        {
            var coordinate = HelperMethods.GetCoordinateOfWallRightOf(playfieldX, playfieldY);
            return VerticalWalls[coordinate.Y, coordinate.W_x];
        }

        public WallSection GetWallRightOf(Player player)
        {
            return GetWallRightOf(player.X, player.Y);
        }
    }
}