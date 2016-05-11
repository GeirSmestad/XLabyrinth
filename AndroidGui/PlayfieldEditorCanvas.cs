using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using LabyrinthEngine.Playfield;
using LabyrinthEngine.LevelConstruction;

namespace AndroidGui
{
    public class PlayfieldEditorCanvas : View
    {
        // Offset values provided by the user by panning & zooming
        private int xOffset = 0;
        private int yOffset = 0;
        private float zoomFactor = 1.0f;

        private float previousX;
        private float previousY;
        private float previousDistanceBetweenTouches;

        private int numFingersOnScreen = 0;

        private const float SCALE_FACTOR = 1.0f;

        private BoardState board;
        private int startX = 75;
        private int startY = 30;
        private int squareHeight = 70;
        private int squareWidth = 70;
        private float wallThickness = 4;

        const int initialSquareHeight = 70;
        const int initialSquareWidth = 70;
        const float initialWallThickness = 4;

        public PlayfieldEditorCanvas(Context context, BoardState board) : base(context)
        {
            this.board = board;

            var paint = new Paint();
            paint.SetARGB(252, 200, 255, 0);

            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 4;
        }


        protected override void OnDraw(Canvas canvas)
        {
            DrawPlayfield(canvas);
        }

        private void DrawPlayfield(Canvas canvas)
        {
            // Horizontal walls
            for (int x = 0; x < board.HorizontalWalls.GetLength(0); x++)
            {
                for (int w_y = 0; w_y < board.HorizontalWalls.GetLength(1); w_y++)
                {
                    Paint paint = new Paint();
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.StrokeWidth = wallThickness;

                    if (board.HorizontalWalls[x, w_y].IsExterior &&
                        board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        paint.SetARGB(255, 180, 180, 0);
                        //pen = new Pen(Color.LightGreen, 4);
                    }
                    else if (board.HorizontalWalls[x, w_y].IsExit &&
                        !board.HorizontalWalls[x, w_y].IsPassable)
                    {
                        paint.SetARGB(255, 0, 180, 180);
                        //pen = new Pen(Color.Yellow, 4);
                    }

                    else if (board.HorizontalWalls[x, w_y].IsExit)
                    {
                        paint.SetARGB(0, 0, 0, 0);
                        // Standard exit, open
                        continue;
                    }
                    else if (board.HorizontalWalls[x, w_y].IsPassable)
                    {
                        paint.SetARGB(0, 0, 0, 0);
                        //pen = new Pen(Color.Black, 1);
                    }
                    else if (board.HorizontalWalls[x, w_y].HasHamster)
                    {
                        paint.SetARGB(255, 180, 180, 0);
                        //pen = new Pen(Color.Green, 4);
                    }
                    else
                    {
                        paint.SetARGB(255, 0, 180, 0);
                        //pen = new Pen(Color.Black, 4);
                    }

                    canvas.DrawLine(
                        startX + squareWidth * x,
                        startY + squareHeight * w_y,
                        startX + squareWidth * x + squareWidth,
                        startY + squareHeight * w_y,
                        paint
                        );

                    paint.Dispose();
                }
            }

            
            // Vertical walls
            for (int y = 0; y < board.VerticalWalls.GetLength(0); y++)
            {
                for (int w_x = 0; w_x < board.HorizontalWalls.GetLength(1); w_x++)
                {
                    Paint paint = new Paint();
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.StrokeWidth = wallThickness;

                    if (board.VerticalWalls[y, w_x].IsExterior &&
                        board.VerticalWalls[y, w_x].HasHamster)
                    {
                        paint.SetARGB(255, 180, 180, 0);
                        //pen = new Pen(Color.LightGreen, 4);
                    }
                    else if (board.VerticalWalls[y, w_x].IsExit &&
                        !board.VerticalWalls[y, w_x].IsPassable)
                    {
                        paint.SetARGB(255, 0, 180, 180);
                        //pen = new Pen(Color.Yellow, 4);
                    }

                    else if (board.VerticalWalls[y, w_x].IsExit)
                    {
                        paint.SetARGB(0, 0, 0, 0);
                        // Standard exit, open
                        continue;
                    }
                    else if (board.VerticalWalls[y, w_x].IsPassable)
                    { 
                        paint.SetARGB(0, 0, 0, 0);
                        //pen = new Pen(Color.Black, 1);
                    }
                    else if (board.VerticalWalls[y, w_x].HasHamster)
                    {
                        paint.SetARGB(255, 180, 180, 0);
                        //pen = new Pen(Color.LightGreen, 4);
                    }
                    else
                    {
                        paint.SetARGB(255, 0, 180, 0);
                        //pen = new Pen(Color.Black, 4);
                    }

                    canvas.DrawLine(
                        startX + squareWidth * w_x,
                        startY + squareHeight * y,
                        startX + squareWidth * w_x,
                        startY + squareHeight * y + squareHeight,
                        paint);
                    paint.Dispose();
                }
            }
            /*
            // Playfield grid
            for (int x = 0; x < board.PlayfieldGrid.GetLength(0); x++)
            {
                for (int y = 0; y < board.PlayfieldGrid.GetLength(1); y++)
                {
                    if (board.PlayfieldGrid[x, y].NumTreasures > 0) // Treasure
                    {
                        e.Graphics.FillRectangle(Brushes.Gold,
                            startX + squareWidth * x + squareWidth / 4,
                            startY + squareHeight * y + squareHeight / 2,
                            squareHeight / 3, squareHeight / 3);
                    }

                    var square = board.PlayfieldGrid[x, y];
                    var drawX = startX + x * squareWidth + squareWidth / 2 - fontSize * 6 / 2;
                    var drawY = startY + y * squareHeight + squareHeight / 2 - fontSize * 6 / 2;

                    var font = new Font(FontFamily.GenericMonospace, fontSize);


                    switch (square.Type)
                    {
                        case SquareType.Empty:
                            break;
                        case SquareType.AmmoStorage:
                            e.Graphics.DrawString("Ammo   ", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.HamsterStorage:
                            e.Graphics.DrawString("Hamster", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.FitnessStudio:
                            e.Graphics.DrawString("Fitness", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.CementStorage:
                            e.Graphics.DrawString("Cement ", font, Brushes.Black, drawX, drawY);
                            break;
                        case SquareType.Teleporter:
                            e.Graphics.DrawString(
                                string.Format("Hole {0} ", square.Hole.TeleporterIndex),
                                font, Brushes.Black, drawX, drawY);
                            break;
                    }

                    font.Dispose();
                }
            }

            var centaur = game.board.centaur;

            // Centaur path
            var centaurPathBrush = new SolidBrush(Color.FromArgb(210, 51, 102, 255));
            var centaurPathPen = new Pen(centaurPathBrush, 3);
            for (int i = 0; i < centaur.Path.Count - 1; i++)
            {
                var from = centaur.Path[i];
                var to = centaur.Path[i + 1];
                e.Graphics.DrawLine(centaurPathPen,
                    startX + squareWidth * from.X + squareWidth / 2,
                    startY + squareHeight * from.Y + squareHeight / 2,
                    startX + squareWidth * to.X + squareWidth / 2,
                    startY + squareHeight * to.Y + squareHeight / 2);
            }

            // Draw line from last to first centaur position, if applicable
            if (centaur.Path.Count >= 2)
            {
                var from = centaur.Path[centaur.Path.Count - 1];
                var to = centaur.Path[0];
                e.Graphics.DrawLine(centaurPathPen,
                    startX + squareWidth * from.X + squareWidth / 2,
                    startY + squareHeight * from.Y + squareHeight / 2,
                    startX + squareWidth * to.X + squareWidth / 2,
                    startY + squareHeight * to.Y + squareHeight / 2);
            }

            centaurPathBrush.Dispose();
            centaurPathPen.Dispose();

            // Centaur
            e.Graphics.FillRectangle(Brushes.Firebrick,
                 startX + squareWidth * centaur.X + squareWidth / 2,
                 startY + squareHeight * centaur.Y + squareHeight / 4,
                 squareHeight / 4, squareHeight / 4);

            // Players
            Brush[] playerBrushes = { Brushes.Blue, Brushes.Green, Brushes.Yellow, Brushes.DarkViolet, Brushes.HotPink, Brushes.DarkCyan };
            Brush[] deadPlayerBrushes = { Brushes.LightBlue, Brushes.LightGreen, Brushes.LightYellow, Brushes.MediumOrchid, Brushes.Pink, Brushes.Cyan };

            for (int playerIndex = 0; playerIndex < game.Players.Count; playerIndex++)
            {
                var player = game.Players[playerIndex];

                Brush brush;
                if (player.IsAlive)
                {
                    brush = playerBrushes[playerIndex];
                }
                else
                {
                    brush = deadPlayerBrushes[playerIndex];
                }

                e.Graphics.FillEllipse(brush,
                    startX + squareWidth * player.X + squareWidth / 2,
                    startY + squareHeight * player.Y + squareHeight / 2,
                    squareHeight / 4, squareHeight / 4);
                
            }
                */
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.Action & e.ActionMasked)
            {
                case MotionEventActions.Move:
                    if (numFingersOnScreen == 1)
                    {
                        return handlePanEvent(e);
                    }
                    else if (numFingersOnScreen == 2)
                    {
                        return handleZoomEvent(e);
                    }
                    break;
                case (MotionEventActions.Down):
                case (MotionEventActions.PointerDown):

                    // Reset tracking of previous finger position
                    previousX = e.GetX();
                    previousY = e.GetY();
                    previousDistanceBetweenTouches = calculateDistanceBetweenTwoTouches(e);

                    if (numFingersOnScreen < 2) { numFingersOnScreen++; }
                    Console.WriteLine("Action was DOWN. NP: {0}", numFingersOnScreen);
                    return true;
                case (MotionEventActions.Up):
                case (MotionEventActions.PointerUp):
                    if (numFingersOnScreen > 0) { numFingersOnScreen--; }
                    Console.WriteLine("Action was UP. NP: {0}", numFingersOnScreen);
                    return true;
                case (MotionEventActions.Cancel):
                    Console.WriteLine("Action was CANCEL");
                    return true;
                case (MotionEventActions.Outside):
                    Console.WriteLine("Movement occurred outside bounds " +
                            "of current screen element");
                    return true;
            }

            return base.OnTouchEvent(e);
        }

        /// <summary>
        /// Handle view panned by single finger
        /// </summary>
        /// <returns>Result of panning action, default true.</returns>
        private bool handlePanEvent(MotionEvent e)
        {
            var dx = e.GetX() - previousX;
            var dy = e.GetY() - previousY;

            previousX = e.GetX(0);
            previousY = e.GetY(0);

            xOffset += (int)(dx * SCALE_FACTOR);
            yOffset += (int)(dy * SCALE_FACTOR);

            startX += (int)(dx * SCALE_FACTOR);
            startY += (int)(dy * SCALE_FACTOR);

            this.Invalidate();

            return true;
        }

        /// <summary>
        /// Handle view zoomed by pinching
        /// </summary>
        /// <returns>Result of zoom action, default true</returns>
        private bool handleZoomEvent(MotionEvent e)
        {
            float dd = calculateDistanceBetweenTwoTouches(e) - previousDistanceBetweenTouches;
            previousDistanceBetweenTouches = calculateDistanceBetweenTwoTouches(e);

            zoomFactor += (dd * 0.001f);

            wallThickness = initialWallThickness * zoomFactor;
            squareHeight = (int)(initialSquareHeight * zoomFactor); // TODO: Convert to float, Android can handle it
            squareWidth = (int)(initialSquareWidth * zoomFactor); // TODO: Convert to float, Android can handle it

            this.Invalidate();

            return true;
        }

        /// <summary>
        /// Calculates the distance between two fingers using Pythagoras' theorem.
        /// Assumes that there are actually two fingers in the motion event.
        /// </summary>
        public float calculateDistanceBetweenTwoTouches(MotionEvent e)
        {
            if (e.PointerCount >= 2)
            {
                float x = e.GetX(0) - e.GetX(1);
                float y = e.GetY(0) - e.GetY(1);

                return (float)Math.Sqrt(x * x + y * y);
            }
            else
            {
                return 0;
            }
        }
    }
}