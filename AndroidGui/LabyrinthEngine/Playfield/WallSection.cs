using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Playfield
{
    public class WallSection
    {
        public WallSection(bool isPassable, bool hasHamster, bool isExit, bool isHiddenExit)
        {
            this.isPassable = isPassable;
            this.hasHamster = hasHamster;
            this.isExit = isExit;
            this.isHiddenExit = isHiddenExit;

            AssertThatWallStateIsLegal();
        }

        private bool isPassable;
        private bool hasHamster;
        private bool isExit;
        private bool isHiddenExit;
        private bool isExterior; // TODO: Unsure if this is required. Implement if so.

        public bool IsPassable
        {
            get
            {
                return isPassable;
            }
            set
            {
                isPassable = value;
                AssertThatWallStateIsLegal();
            }
        }

        public bool HasHamster
        {
            get
            {
                return hasHamster;
            }
            set
            {
                hasHamster = value;
                AssertThatWallStateIsLegal();
            }
        }
        public bool IsExit
        {
            get
            {
                return isExit;
            }
            set
            {
                isExit = value;
                AssertThatWallStateIsLegal();
            }
        }
        public bool IsHiddenExit
        {
            get
            {
                return isHiddenExit;
            }
            set
            {
                isHiddenExit = value;
                AssertThatWallStateIsLegal();
            }
        }

        private void AssertThatWallStateIsLegal()
        {
            if (IsExit && IsHiddenExit)
            {
                throw new InvalidOperationException("A wall section cannot both be exit and hidden exit");
            }

            if (IsPassable && HasHamster)
            {
                throw new InvalidOperationException("A wall section cannot both have a hamster and be passable");
            }
        }
    }
}