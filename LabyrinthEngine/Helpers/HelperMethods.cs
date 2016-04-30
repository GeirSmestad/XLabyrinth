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

        public static List<Teleporter> PopulateTeleportersFrom(PlayfieldSquare[,] playfield)
        {
            var result = new List<Teleporter>();

            for (int x = 0; x < playfield.GetLength(0); x++)
            {
                for (int y = 0; y < playfield.GetLength(1); y++)
                {
                    var playfieldSquare = playfield[x, y];
                    if (playfieldSquare.Type == SquareType.Teleporter)
                    {
                        result.Add(playfieldSquare.Hole);
                    }
                }
            }

            result.Sort((teleporter1, teleporter2) =>
                teleporter1.TeleporterIndex.CompareTo(teleporter2.TeleporterIndex));

            if (result.Count == 0)
            {
                return result;
            }
            else if (result.Count == 1)
            {
                result[0].NextHole = result[0];
                return result;
            }

            for (int teleporterIndex = 0; teleporterIndex < result.Count - 1; teleporterIndex++)
            {
                result[teleporterIndex].NextHole = result[teleporterIndex + 1];
            }
            result[result.Count - 1].NextHole = result[0];

            return result;
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
