using LabyrinthEngine.Playfield;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LabyrinthEngine.LevelConstruction
{
    /// <summary>
    /// Saves a HamsterLabyrinth level from its in-game representation to a legal XML
    /// representation. Result is human-readable, but may not be the most efficient
    /// representation possible. (E.g. *all* exterior walls are included in the saved
    /// file, even though walls on the border of the playfield will be marked as exterior
    /// and unpassable in the game regardless of whether included in the XML file or not).
    /// </summary>
    public class BoardSaver
    {
        private BoardState source;

        private XmlDocument document;
        private XmlElement levelElement;

        /// <summary>
        /// To save a level, pass a legal instance of the BoardState class to this constructor,
        /// then access the result through this class's public interface.
        /// </summary>
        public BoardSaver(BoardState boardToSave)
        {
            source = boardToSave;
        }

        /// <summary>
        /// Generates a legal XML representation of the BoardState given to the constructor.
        /// </summary>
        public string GenerateXmlRepresentationOfBoard()
        {
            document = new XmlDocument();

            levelElement = (XmlElement)document.AppendChild(document.CreateElement("LabyrinthLevel"));

            generatePlayfieldElement();
            generateHorizontalWallsElement();
            generateVerticalWallsElement();
            generateStartingPositionsElement();
            generateCentaurElement();

            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = XmlWriter.Create(stringWriter))
                {
                    document.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }

        /// <summary>
        /// Saves to file an XML representation of the BoardState given to the constructor.
        /// </summary>
        public void SaveXmlToFile(string filename)
        {
            if (document == null)
            {
                GenerateXmlRepresentationOfBoard();
            }

            document.Save(filename);
        }

        private void generatePlayfieldElement()
        {
            var playfieldElement = (XmlElement)levelElement
                .AppendChild(document.CreateElement("Playfield"));
            playfieldElement.SetAttribute("width", source.Width.ToString());
            playfieldElement.SetAttribute("height", source.Height.ToString());

            for (int y = 0; y < source.Height; y++)
            {
                var rowElement = (XmlElement)playfieldElement
                    .AppendChild(document.CreateElement("Row"));
                for (int x = 0; x < source.Width; x++)
                {
                    var square = source.PlayfieldGrid[x, y];
                    string squareType = square.Type.ToString();

                    var squareElement = (XmlElement)rowElement
                        .AppendChild(document.CreateElement(squareType));
                    if (square.NumTreasures != 0)
                    {
                        squareElement.SetAttribute("treasures", square.NumTreasures.ToString());
                    }

                    if (square.Hole != null)
                    {
                        squareElement.SetAttribute("teleporterIndex", square.Hole.TeleporterIndex.ToString());
                    }
                }
            }
        }

        private void generateHorizontalWallsElement()
        {
            var horizontalWallsElement = (XmlElement)levelElement
                .AppendChild(document.CreateElement("HorizontalWalls"));

            for (int x = 0; x < source.Width; x++)
            {
                for (int w_y = 0; w_y < source.Height+1; w_y++)
                {
                    var wallSegment = source.HorizontalWalls[x, w_y];
                    var attributes = new List<string>();
                    if (wallSegment.HasHamster)
                    {
                        attributes.Add("hasHamster");
                    }
                    if (wallSegment.IsExit)
                    {
                        attributes.Add("isExit");
                    }
                    if (wallSegment.IsExterior)
                    {
                        attributes.Add("isExterior");
                    }

                    if (attributes.Count == 0 && wallSegment.IsPassable) { continue; }

                    var wallElement = (XmlElement)horizontalWallsElement
                            .AppendChild(document.CreateElement("HorizontalWallSegment"));
                    wallElement.SetAttribute("w_y", w_y.ToString());
                    wallElement.SetAttribute("x", x.ToString());
                    foreach (var attribute in attributes)
                    {
                        wallElement.SetAttribute(attribute, "yes");
                    }

                    if (wallSegment.IsPassable)
                    {
                        wallElement.SetAttribute("isPassable", "yes");
                    }
                }
            }
        }

        private void generateVerticalWallsElement()
        {
            var verticalWallsElement = (XmlElement)levelElement
                .AppendChild(document.CreateElement("VerticalWalls"));

            for (int y = 0; y < source.Width; y++)
            {
                for (int w_x = 0; w_x < source.Height + 1; w_x++)
                {
                    var wallSegment = source.VerticalWalls[y, w_x];
                    var attributes = new List<string>();
                    if (wallSegment.HasHamster)
                    {
                        attributes.Add("hasHamster");
                    }
                    if (wallSegment.IsExit)
                    {
                        attributes.Add("isExit");
                    }
                    if (wallSegment.IsExterior)
                    {
                        attributes.Add("isExterior");
                    }

                    if (attributes.Count == 0 && wallSegment.IsPassable) { continue; }

                    var wallElement = (XmlElement)verticalWallsElement
                            .AppendChild(document.CreateElement("VerticalWallSegment"));
                    wallElement.SetAttribute("w_x", w_x.ToString());
                    wallElement.SetAttribute("y", y.ToString());
                    foreach (var attribute in attributes)
                    {
                        wallElement.SetAttribute(attribute, "yes");
                    }

                    if (wallSegment.IsPassable)
                    {
                        wallElement.SetAttribute("isPassable", "yes");
                    }
                }
            }
        }

        private void generateStartingPositionsElement()
        {
            var startingPositionsElement = (XmlElement)levelElement
                .AppendChild(document.CreateElement("StartingPositions"));

            foreach (var startingPosition in source.StartingPositions)
            {
                var playerPositionElement = (XmlElement)startingPositionsElement
                    .AppendChild(document.CreateElement("PlayerPosition"));

                playerPositionElement.SetAttribute("x", startingPosition.X.ToString());
                playerPositionElement.SetAttribute("y", startingPosition.Y.ToString());
            }
        }

        private void generateCentaurElement()
        {
            var centaurElement = (XmlElement)levelElement
                .AppendChild(document.CreateElement("Centaur"));
            centaurElement.SetAttribute("startX", source.centaur.X.ToString());
            centaurElement.SetAttribute("startY", source.centaur.Y.ToString());

            foreach (var step in source.centaur.Path)
            {
                var stepElement = (XmlElement)centaurElement
                    .AppendChild(document.CreateElement("CentaurStep"));
                stepElement.SetAttribute("x", step.X.ToString());
                stepElement.SetAttribute("y", step.Y.ToString());

                if (step.IgnoreWallsWhenSteppingHere)
                {
                    stepElement.SetAttribute("stepHereIgnoringWalls", "yes");
                }
            }
        }
    }
}
