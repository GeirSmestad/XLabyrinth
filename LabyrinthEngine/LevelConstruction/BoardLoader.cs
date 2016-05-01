using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Playfield;
using System.Xml;
using System.Xml.XPath;
using LabyrinthEngine.Entities;
using LabyrinthEngine.Helpers;
using LabyrinthEngine.Geometry;

namespace LabyrinthEngine.LevelConstruction
{
    /// <summary>
    /// Loads a HamsterLabyrinth level from an XML string. See the test project for an annotated example
    /// file. (E.g. BoardLoaderTestBoard.xml).
    /// </summary>
    public class BoardLoader
    {
        /// <summary>
        /// The board state representing the XML string given to the constructor
        /// </summary>
        public BoardState Board { get; private set; }

        private int boardWidth;
        private int boardHeight;

        private PlayfieldSquare[,] playfield;

        /// <summary>
        /// To load a level, pass a legel XML description of a HamsterLabyrinth level to this constructor,
        /// then read the result out from the Board property.
        /// </summary>
        /// <param name="xmlToInitializeBoardFrom">A level XML string representing the level to load</param>
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
            playfield = parsePlayfieldFrom(playfieldXml);

            var horizontalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/HorizontalWalls");
            var horizontalWalls = parseHorizontalWallsFrom(horizontalWallsXml);

            var verticalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/VerticalWalls");
            var verticalWalls = parseVerticalWallsFrom(verticalWallsXml);

            var centaurXml = navigator.SelectSingleNode("/LabyrinthLevel/Centaur");
            var centaur = parseCentaurFrom(centaurXml);

            var startingPositionsXml = navigator.SelectSingleNode("/LabyrinthLevel/StartingPositions");
            var startingPositions = parseStartingPositionsFrom(startingPositionsXml);

            List<Teleporter> holes = HelperMethods.PopulateTeleportersFrom(playfield);
            populatePlayfieldCoordinates();

            return new BoardState(playfield, horizontalWalls, verticalWalls, holes, 
                centaur, startingPositions);
        }

        private void populatePlayfieldCoordinates()
        {
            for (int x = 0; x < playfield.GetLength(0); x++)
            {
                for (int y = 0; y < playfield.GetLength(1); y++)
                {
                    var playfieldSquare = playfield[x, y];
                    playfieldSquare.X = x;
                    playfieldSquare.Y = y;
                }
            }
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
                        result[x, y] = parsePlayfieldSquareFrom(currentPlayfieldSquare,
                            withXCoordinate: x, andYCoordinate:y);
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

        private PlayfieldSquare parsePlayfieldSquareFrom(XPathNavigator playfieldSquareElement,
            int withXCoordinate, int andYCoordinate)
        {
            int x = withXCoordinate;
            int y = andYCoordinate;
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
                // Teleporters are converted to linked list once all have been parsed, afterwards.
                var index = int.Parse(playfieldSquareElement.GetAttribute("teleporterIndex"));
                teleporter = new Teleporter(index, null, x, y);

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

        private Centaur parseCentaurFrom(XPathNavigator centaurXml)
        {
            int startX;
            int startY;

            try
            {
                startX = int.Parse(centaurXml.GetAttribute("startX"));
                startY = int.Parse(centaurXml.GetAttribute("startY"));
            }
            catch (Exception)
            {
                throw new LabyrinthParseException("Error when parsing starting position for centaur");
            }

            var centaurPath = new List<CentaurStep>();
            try
            {
                var iteratorForAllCentaurSteps = centaurXml.Select("CentaurStep");
                while (iteratorForAllCentaurSteps.MoveNext())
                {
                    var currentCentaurStepElement = iteratorForAllCentaurSteps.Current.Clone();

                    int x = int.Parse(currentCentaurStepElement.GetAttribute("x"));
                    int y = int.Parse(currentCentaurStepElement.GetAttribute("y"));
                    bool ignoreWallsWhenSteppingHere = currentCentaurStepElement
                        .HasAttributeEqualTo("stepHereIgnoringWalls", "yes");

                    centaurPath.Add(new CentaurStep(x, y, ignoreWallsWhenSteppingHere));
                }
            }
            catch (Exception)
            {
                throw new LabyrinthParseException("Encountered illegal centaur step element");
            }

            return new Centaur(startX, startY, centaurPath);
        }
    }
}