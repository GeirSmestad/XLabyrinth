using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AndroidGui.Core.Playfield;

namespace AndroidGui.Core.Helpers
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
            throw new NotImplementedException();
        }
    }
}