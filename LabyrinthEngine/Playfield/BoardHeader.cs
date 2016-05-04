using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthEngine.Playfield
{
    [Serializable]
    public class BoardHeader
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }

        public BoardHeader()
        {
            Name = "No name";
            Description = "No description";
        }
    }
}
