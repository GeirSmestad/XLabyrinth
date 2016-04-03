using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Playfield
{
    public class WallSection
    {
        public WallSection(bool isPassable, bool hasHamster, 
            bool isExit, bool isExterior)
        {
            this.isPassable = isPassable;
            this.hasHamster = hasHamster;
            this.isExit = isExit;
            this.isExterior = isExterior;

            AssertThatWallStateIsLegal();
        }

        private bool isPassable;
        private bool hasHamster;
        private bool isExit;
        private bool isExterior;

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

        public bool IsExterior
        {
            get
            {
                return isExterior;
            }
            set
            {
                isExterior = value;
                AssertThatWallStateIsLegal();
            }
        }

        private void AssertThatWallStateIsLegal()
        {
            if (IsExit && !IsExterior)
            {
                throw new InvalidOperationException("The exit must be on an exterior wall");
            }

            if (IsPassable && HasHamster)
            {
                throw new InvalidOperationException("A wall section cannot both have a hamster and be passable");
            }

        }
    }
}