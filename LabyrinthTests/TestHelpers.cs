using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthTests
{
    public class TestHelpers
    {

        public static PlayfieldSquare[,] InitializeEmptyPlayfield(int boardWidth, int boardHeight)
        {
            var result = new PlayfieldSquare[boardWidth, boardHeight];

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    result[x, y] = new PlayfieldSquare(SquareType.Empty, 0);
                }
            }
            return result;
        }

        public static WallSection[,] InitializeEmptyHorizontalWalls(int boardWidth, int boardHeight)
        {
            var result = new WallSection[boardWidth, boardHeight + 1];

            for (int w_y = 0; w_y <= boardHeight; w_y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    if (w_y == 0 || w_y == boardHeight)
                    {
                        result[x, w_y] = new WallSection(false, false, false, isExterior: true);
                    }
                    else
                    {
                        result[x, w_y] = new WallSection(true, false, false, false); // No wall
                    }
                }
            }
            return result;
        }

        public static WallSection[,] InitializeEmptyVerticalWalls(int boardWidth, int boardHeight)
        {
            var result = new WallSection[boardWidth, boardHeight + 1];

            for (int w_x = 0; w_x <= boardWidth; w_x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (w_x == 0 || w_x == boardWidth)
                    {
                        result[y, w_x] = new WallSection(false, false, false, isExterior: true);
                    }
                    else
                    {
                        result[y, w_x] = new WallSection(true, false, false, false); // No wall
                    }
                }
            }
            return result;
        }
    }
}
