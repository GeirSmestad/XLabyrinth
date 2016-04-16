using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Helpers
{
    class LabyrinthParseException : Exception
    {
        public LabyrinthParseException() { }
        public LabyrinthParseException(string message) : base(message) { }
        public LabyrinthParseException(string message, Exception inner) : base(message, inner ) { }
    }

    class LabyrinthInvalidStateException : Exception
    {
        public LabyrinthInvalidStateException() { }
        public LabyrinthInvalidStateException(string message) : base(message) { }
        public LabyrinthInvalidStateException(string message, Exception inner) : base(message, inner) { }
    }
}