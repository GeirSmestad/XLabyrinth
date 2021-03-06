using LabyrinthEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LabyrinthEngine.Playfield
{
    [Serializable]
    public class WallSection : IEquatable<object>
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
                throw new LabyrinthInvalidStateException("The exit must be on an exterior wall");
            }

            if (IsPassable && HasHamster)
            {
                throw new LabyrinthInvalidStateException("A wall section cannot both have a hamster and be passable");
            }

        }

        public override bool Equals(object item)
        {
            var other = item as WallSection;

            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            var result = true;
            result &= isPassable == other.isPassable && hasHamster == other.hasHamster;
            result &= isExit == other.IsExit && isExterior == other.isExterior;

            return result;
        }
    }
}