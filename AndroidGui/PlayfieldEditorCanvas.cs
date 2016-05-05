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

namespace AndroidGui
{
    public class PlayfieldEditorCanvas : View
    {
        private readonly ShapeDrawable _shape;

        // Offset values provided by the user by panning & zooming
        private int xOffset = 0;
        private int yOffset = 0;
        private float zoomFactor = 1.0f;

        private float previousX;
        private float previousY;
        private float previousDistanceBetweenTouches;

        private int numFingersOnScreen = 0;

        private const float SCALE_FACTOR = 1.0f;

        public PlayfieldEditorCanvas(Context context) : base(context)
        {
            var paint = new Paint();
            paint.SetARGB(252, 200, 255, 0);

            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 4;

            _shape = new ShapeDrawable(new OvalShape());
            _shape.Paint.Set(paint);

            _shape.SetBounds(20 + xOffset, 20 + yOffset, 
                (int)((300 + xOffset) * zoomFactor), 
                (int)((200 + yOffset) * zoomFactor));
        }


        protected override void OnDraw(Canvas canvas)
        {
            _shape.Draw(canvas);
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

            _shape.SetBounds(20 + xOffset, 20 + yOffset, 
                (int)((300 + xOffset) * zoomFactor), 
                (int)((200 + yOffset) * zoomFactor));
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
            _shape.SetBounds(20 + xOffset, 20 + yOffset,
                (int)((300 + xOffset) * zoomFactor),
                (int)((200 + yOffset) * zoomFactor));
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