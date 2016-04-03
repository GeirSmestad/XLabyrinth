using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabyrinthEngine.Playfield;
using System.Xml;
using System.Xml.XPath;

namespace LabyrinthEngine.Helpers
{
    public class BoardLoader
    {
        /* TEMP:
         * To simplify wall specification in XML, we could assume that
         * 1) All exterior walls are impassable except if exit
         * 2) No interior walls unless specified
         * 
         * Walls are completely specified in the binary format and in memory,
         * but the XML format is more lenient.
         * 
        */

        public static BoardState InitializeFromXml(string xml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            XPathNavigator navigator = document.CreateNavigator();

            var playfieldXml = navigator.Compile("/LabyrinthLevel/Playfield/Row");

            XPathNodeIterator iterator = navigator.Select(playfieldXml);

            while(iterator.MoveNext())
            {
                XPathNavigator element = iterator.Current.Clone();

            }
            
            return null;
        }
    }
}