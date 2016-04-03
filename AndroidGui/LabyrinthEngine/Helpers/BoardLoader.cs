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
        public static BoardState InitializeFromXml(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            var navigator = xmlDocument.CreateNavigator();

            var playfieldXml = navigator.SelectSingleNode("/LabyrinthLevel/Playfield");
            var playfield = parsePlayfieldFrom(playfieldXml);

            WallSection[,] horizontalWalls = null;
            var horizontalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/HorizontalWalls");

            WallSection[,] verticalWalls = null;
            var verticalWallsXml = navigator.SelectSingleNode("/LabyrinthLevel/VerticalWalls");

            // TODO: Add code to read and insert exits into wall definitions (LabyrinthExits)
            // TODO: Add code to construct teleporters from already-parsed playfield
            // TODO: Populate playfield X and Y coordinates from already-parsed playfield

            List<Teleporter> holes = null;

            Centaur centaur = null;
            var centaurXml = navigator.SelectSingleNode("/LabyrinthLevel/Centaur");

            // TODO: Add code to read and store StartingPositions to BoardState

            return new BoardState(playfield, horizontalWalls, verticalWalls, holes, centaur);
        }

        private static PlayfieldSquare[,] parsePlayfieldFrom(XPathNavigator playfieldElement)
        {
            int width = int.Parse(playfieldElement.GetAttribute("width"));
            int height = int.Parse(playfieldElement.GetAttribute("height"));

            PlayfieldSquare[,] result = new PlayfieldSquare[width, height];

            try
            {
                var iteratorForAllRows = playfieldElement.Select("Row");

                for (int y = 0; y < height; y++)
                {
                    iteratorForAllRows.MoveNext();
                    var currentRowElement = iteratorForAllRows.Current.Clone();
                    var iteratorForAllSquares = currentRowElement.Select("*");

                    for (int x = 0; x < width; x++)
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

        private static PlayfieldSquare parsePlayfieldSquareFrom(XPathNavigator playfieldSquareElement)
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

                //var index = int.Parse(playfieldSquareElement.GetAttribute("teleporterIndex"));
                // TODO: Fix this. Doesn't work because of circular dependency in constructors
                //teleporter = new Teleporter(index, null, result);
                //return new PlayfieldSquare(type, numTreasures, teleporter);
            }

            return new PlayfieldSquare(type, numTreasures);
        }
    }
}