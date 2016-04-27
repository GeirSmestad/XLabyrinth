using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Helpers
{
    public class BoardComparer
    {
        public static bool Equals(BoardState a, BoardState b)
        {
            if (a.Height != b.Height || a.Width != b.Width)
            {
                return false;
            }

            try
            {
                for (int x = 0; x < a.Width; x++)
                {
                    for (int y = 0; y < a.Height; y++)
                    {
                        if (!a.PlayfieldGrid[x, y].Equals(b.PlayfieldGrid[x, y]))
                        {
                            return false;
                        }
                    }
                }

                for (int y = 0; y < a.Width; y++)
                {
                    for (int w_x = 0; w_x < a.Height + 1; w_x++)
                    {
                        if (!a.VerticalWalls[y, w_x].Equals(b.VerticalWalls[y, w_x]))
                        {
                            return false;
                        }
                    }
                }

                for (int x = 0; x < a.Width; x++)
                {
                    for (int w_y = 0; w_y < a.Height + 1; w_y++)
                    {
                        if (!a.HorizontalWalls[x, w_y].Equals(b.HorizontalWalls[x, w_y]))
                        {
                            return false;
                        }
                    }
                }

                if (a.Holes != null)
                {
                    for (int i = 0; i < a.Holes.Count; i++)
                    {
                        if (!a.Holes[i].Equals(b.Holes[i]))
                        {
                            return false;
                        }
                    }
                }

                if (!a.centaur.Equals(b.centaur))
                {
                    return false;
                }

                if (a.StartingPositions != null)
                {
                    for (int i = 0; i < a.StartingPositions.Count; i++)
                    {
                        if (!a.StartingPositions[i].Equals(b.StartingPositions[i]))
                        {
                            return false;
                        }
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }

            return true; // This is the only case in which a and b are equal
        }
    }
}
