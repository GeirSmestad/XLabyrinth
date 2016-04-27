using LabyrinthEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace LabyrinthEngine.Entities
{
    [Serializable]
    public class Centaur
    {
        private int startX { get; set; }
        private int startY { get; set; }
        public ReadOnlyCollection<CentaurStep> Path { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int NextStep { get; private set; }
        public bool CurrentlyBacktracking { get; private set; }
        public bool HasStartedMoving { get; private set; }

        /// <summary>
        /// The centaur starts at (startX, startY), moving to the first element of Path the
        /// first time Move is called. On subsequent calls, it steps through Path from the
        /// first to the last element and then resumes from the beginning of Path. This class
        /// does not take walls into consideration, and consumers need to choose whether to call
        /// Move or not.
        /// </summary>
        public Centaur(int startX, int startY, List<CentaurStep> path)
        {
            this.startX = startX;
            this.startY = startY;
            Path = path.AsReadOnly();

            resetPosition();
        }

        public void ReverseDirection()
        {
            CurrentlyBacktracking = !CurrentlyBacktracking;

            if (CurrentlyBacktracking)
            {
                NextStep = HelperMethods.PositiveModulo((NextStep - 2), Path.Count);
            }
            else
            {
                NextStep = (NextStep + 2) % Path.Count;
            }
        }

        public CentaurStep NextPositionInPath()
        {
            if (!HasStartedMoving)
            {
                if (Path.Count > 0)
                {
                    return Path[0];
                }
                else
                {
                    return new CentaurStep(X, Y, false);
                }
            }

            if (Path.Count == 0)
            {
                return new CentaurStep(X, Y, false);
            }
            
            else if (!CurrentlyBacktracking)
            {
                var result = Path[NextStep];
                return result;
            }
            else
            {
                var result = Path[NextStep];
                return result;
            }
        }

        /// <summary>
        /// Moves the centaur to its next position, not taking walls into consideration.
        /// Whomever calls Move has to decide whether the centaur *should* move or not.
        /// </summary>
        public void MoveToNextPositionInPath()
        {
            if (Path.Count == 0)
            {
                return;
            }

            var positionAfterMoving = NextPositionInPath();

            X = positionAfterMoving.X;
            Y = positionAfterMoving.Y;

            if (!HasStartedMoving)
            {
                HasStartedMoving = true;
            }

            if (!CurrentlyBacktracking)
            {
                NextStep = (NextStep + 1) % Path.Count;
            }
            else
            {
                NextStep = HelperMethods.PositiveModulo((NextStep - 1), Path.Count);
            }
        }

        private void resetPosition()
        {
            X = startX;
            Y = startY;
            NextStep = 0;
            CurrentlyBacktracking = false;
            HasStartedMoving = false;
        }

        public override bool Equals(object item)
        {
            var other = item as PlayfieldSquare;

            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return false; // TODO: Implement proper equality test
        }
    }
}