using LabyrinthEngine.Entities;
using LabyrinthEngine.Geometry;
using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Helpers
{
    public class HelperMethods
    {
        /// <summary>
        /// Returns a deep copy of the specified object.
        /// </summary>
        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static bool AreAdjacent(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)
            {
                if (y1 == y2 - 1 || y1 == y2 + 1)
                {
                    return true;
                }
            }
            else if (y1 == y2)
            {
                if (x1 == x2 - 1 || x1 == x2 + 1)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NextMoveOfCentaurIsBlockedByWall(Centaur centaur, BoardState board)
        {
            var nextCentaurPosition = centaur.NextPositionInPath();

            if (centaur.X == nextCentaurPosition.X)
            {
                if (centaur.Y < nextCentaurPosition.Y)
                {
                    return !board.GetWallBelow(centaur.X, centaur.Y).IsPassable;
                }
                else // centaur.Y > nextCentaurPosition.Y
                {
                    return !board.GetWallAbove(centaur.X, centaur.Y).IsPassable;
                }
            }
            else // centaur.Y == nextCentaurPosition.Y
            {
                if (centaur.X < nextCentaurPosition.X)
                {
                    return !board.GetWallRightOf(centaur.X, centaur.Y).IsPassable;
                }
                else // centaur.X > nextCentaurPosition.X
                {
                    return !board.GetWallLeftOf(centaur.X, centaur.Y).IsPassable;
                }
            }
        }

        public static int PositiveModulo(int x, int m)
        {
            return (x % m + m) % m;
        }

        public static VerticalWallCoordinate GetCoordinateOfWallLeftOf(int x, int y)
        {
            return new VerticalWallCoordinate(y, x);
        }

        public static VerticalWallCoordinate GetCoordinateOfWallRightOf(int x, int y)
        {
            return new VerticalWallCoordinate(y, x + 1);
        }

        public static HorizontalWallCoordinate GetCoordinateOfWallAbove(int x, int y)
        {
            return new HorizontalWallCoordinate(x, y);
        }

        public static HorizontalWallCoordinate GetCoordinateOfWallBelow(int x, int y)
        {
            return new HorizontalWallCoordinate(x, y + 1);
        }
    }
}
