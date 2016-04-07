using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace LabyrinthEngine.Entities
{
    [Serializable]
    public class Centaur
    {
        private int startX { get; set; }
        private int startY { get; set; }
        public List<CentaurStep> Path { get; private set; }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int CurrentStep { get; private set; }
        public bool CurrentlyBacktracking { get; private set; }


        public Centaur(int startX, int startY, List<CentaurStep> path)
        {
            this.startX = startX;
            this.startY = startY;
            Path = path;

            ResetPosition();
        }

        public void ResetPosition()
        {
            X = startX;
            Y = startY;
            CurrentStep = 0;
            CurrentlyBacktracking = false;
        }

        public void ReverseDirection()
        {
            CurrentlyBacktracking = !CurrentlyBacktracking;
        }

        /// <summary>
        /// Moves the centaur to its next position, not taking walls into consideration.
        /// </summary>
        public void Move()
        {
            if (CurrentlyBacktracking)
            {

            } else
            {

            }
        }
    }
}