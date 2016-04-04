using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Playfield;
using System.Xml;
using System.Xml.XPath;
using LabyrinthEngine.Entities;

namespace LabyrinthEngine.Helpers
{
    public class BoardLoader
    {
        public BoardState Board { get; private set; }

        private int boardWidth;
        private int boardHeight;

        public BoardLoader(string xmlToInitializeBoardFrom)
        {
            Board = initializeBoardFromXml(xmlToInitializeBoardFrom);
        }

        private BoardState initializeBoardFromXml(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            var navigator = xmlDocument.CreateNavigator();

            var playfieldXml = navigator.SelectSingleNode("/LabyrinthLevel/Playfield");
            var playfield = parsePlayfieldFrom(playfieldXml);

            var horizontalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/HorizontalWalls");
            var horizontalWalls = parseHorizontalWallsFrom(horizontalWallsXml);

            var verticalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/VerticalWalls");
            var verticalWalls = parseVerticalWallsFrom(verticalWallsXml);

            // TODO: Add code to construct teleporters from already-parsed playfield
            // TODO: Populate playfield X and Y coordinates from already-parsed playfield

            List<Teleporter> holes = null;

            Centaur centaur = null;
            var centaurXml = navigator.SelectSingleNode("/LabyrinthLevel/Centaur");

            var startingPositionsXml = navigator.SelectSingleNode("/LabyrinthLevel/StartingPositions");
            var startingPositions = parseStartingPositionsFrom(startingPositionsXml);

            return new BoardState(playfield, horizontalWalls, verticalWalls, holes, 
                centaur, startingPositions);
        }

        private PlayfieldSquare[,] parsePlayfieldFrom(XPathNavigator playfieldElement)
        {
            boardWidth = int.Parse(playfieldElement.GetAttribute("width"));
            boardHeight = int.Parse(playfieldElement.GetAttribute("height"));

            PlayfieldSquare[,] result = new PlayfieldSquare[boardWidth, boardHeight];

            try
            {
                var iteratorForAllRows = playfieldElement.Select("Row");

                for (int y = 0; y < boardHeight; y++)
                {
                    iteratorForAllRows.MoveNext();
                    var currentRowElement = iteratorForAllRows.Current.Clone();
                    var iteratorForAllSquares = currentRowElement.Select("*");

                    for (int x = 0; x < boardWidth; x++)
                    {
                        iteratorForAllSquares.MoveNext();
                        var currentPlayfieldSquare = iteratorForAllSquares.Current.Clone();
                        result[x, y] = parsePlayfieldSquareFrom(currentPlayfieldSquare);
                    }
                }
            }
            catch (Exception e)
            {
                if (e.GetType() != typeof(LabyrinthParseException))
                {
                    throw new LabyrinthParseException("Error when parsing playfield. "
                        + "Do board dimensions match playfield definition?");
                } else
                {
                    throw;
                }
            }

            return result;
        }

        private PlayfieldSquare parsePlayfieldSquareFrom(XPathNavigator playfieldSquareElement)
        {
            int numTreasures = 0;
            SquareType type;
            Teleporter teleporter;

            if (!string.IsNullOrEmpty(playfieldSquareElement.GetAttribute("treasures")))
            {
                numTreasures = int.Parse(playfieldSquareElement.GetAttribute("treasures"));
            }

            var squareElementName = playfieldSquareElement.Name;
            bool elementNameIsValid = Enum.TryParse<SquareType>(squareElementName, true, out type);
            if (!elementNameIsValid)
            {
                throw new LabyrinthParseException("Encountered unknown element "
                    + "when parsing playfield square: " + playfieldSquareElement.Name);
            }

            if (type == SquareType.Teleporter)
            {
                // Teleporters must be converted to a linked list once all have been parsed.

                var index = int.Parse(playfieldSquareElement.GetAttribute("teleporterIndex"));
                teleporter = new Teleporter(index, null);
                return new PlayfieldSquare(type, numTreasures, teleporter);
            }

            return new PlayfieldSquare(type, numTreasures);
        }

        private WallSection[,] parseHorizontalWallsFrom(XPathNavigator horizontalWallsXml)
        {
            var result = new WallSection[boardWidth, boardHeight+1];

            for (int w_y = 0; w_y <= boardHeight; w_y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    if (w_y == 0 || w_y == boardHeight)
                    {
                        result[x, w_y] = new WallSection(false, false, false, isExterior:true);
                    }
                    else
                    { 
                        result[x, w_y] = new WallSection(true, false, false, false); // No wall
                    }
                }
            }

            var iteratorForAllHorizontalWalls = horizontalWallsXml.Select("HorizontalWallSegment");
            while (iteratorForAllHorizontalWalls.MoveNext())
            {
                var currentWallElement = iteratorForAllHorizontalWalls.Current.Clone();

                int w_y, x;
                var currentWall = parseWallSectionFrom(currentWallElement, out w_y, out x);

                result[x, w_y] = currentWall;
            }

            return result;
        }

        private WallSection[,] parseVerticalWallsFrom(XPathNavigator verticalWallsXml)
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

            var iteratorForAllVerticalWalls = verticalWallsXml.Select("VerticalWallSegment");
            while (iteratorForAllVerticalWalls.MoveNext())
            {
                var currentWallElement = iteratorForAllVerticalWalls.Current.Clone();

                int w_x, y;
                var currentWall = parseWallSectionFrom(currentWallElement, out w_x, out y);

                result[y, w_x] = currentWall;
            }

            return result;
        }

        private WallSection parseWallSectionFrom(XPathNavigator wallSectionXml, 
            out int wallCoordinate, out int adjacentSquareCoordinate)
        {
            bool isPassable = wallSectionXml.HasAttributeEqualTo("isPassable", "yes");
            bool isExit = wallSectionXml.HasAttributeEqualTo("isExit", "yes");
            bool hasHamster = wallSectionXml.HasAttributeEqualTo("hasHamster", "yes");
            bool isExterior;

            if (wallSectionXml.Name == "HorizontalWallSegment")
            {
                wallCoordinate = int.Parse(wallSectionXml.GetAttribute("w_y"));
                adjacentSquareCoordinate = int.Parse(wallSectionXml.GetAttribute("x"));
                isExterior = wallCoordinate == 0 || wallCoordinate == boardHeight;
            }
            else if (wallSectionXml.Name == "VerticalWallSegment")
            {
                wallCoordinate = int.Parse(wallSectionXml.GetAttribute("w_x"));
                adjacentSquareCoordinate = int.Parse(wallSectionXml.GetAttribute("y"));
                isExterior = wallCoordinate == 0 || wallCoordinate == boardWidth;
            }
            else
            {
                throw new LabyrinthParseException("Encountered illegal wall segment name");
            }

            return new WallSection(isPassable, hasHamster, isExit, isExterior);
        }

        private List<Position> parseStartingPositionsFrom(XPathNavigator startingPositionsXml)
        {
            var result = new List<Position>();

            var iteratorForAllStartingPositions = startingPositionsXml.Select("PlayerPosition");
            while (iteratorForAllStartingPositions.MoveNext())
            {
                var currentStartingPositionElement = iteratorForAllStartingPositions.Current.Clone();

                try
                {
                    var x = int.Parse(currentStartingPositionElement.GetAttribute("x"));
                    var y = int.Parse(currentStartingPositionElement.GetAttribute("y"));

                    result.Add(new Position(x, y));
                }
                catch (Exception)
                {
                    throw new LabyrinthParseException("Encountered illegal starting position element");
                }
            }

            return result;
        }
    }
}