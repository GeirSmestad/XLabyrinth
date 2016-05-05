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
        private int x = 0;
        private int y = 0;

        private int distanceModifier = 0;

        private float previousX = -1;
        private float previousY = -1;

        private float previousSecondFingerX = -1;
        private float previousSecondFingerY = -1;

        private float previousDistanceBetweenTouches = -1;

        private int numPointers = 0;

        private const float SCALE_FACTOR = 1.0f;

        public PlayfieldEditorCanvas(Context context) : base(context)
        {
            var paint = new Paint();
            paint.SetARGB(252, 200, 255, 0);

            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 4;

            _shape = new ShapeDrawable(new OvalShape());
            _shape.Paint.Set(paint);

            _shape.SetBounds(20 + x, 20 + y, 300 + x + distanceModifier, 200 + y + distanceModifier);
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
                    if (numPointers == 1)
                    {
                        return handlePanEvent(e);
                    }
                    else if (numPointers == 2)
                    {
                        if (previousSecondFingerX == -1) { previousSecondFingerX = e.GetX(1); }
                        if (previousSecondFingerY == -1) { previousSecondFingerY = e.GetY(1); }

                        if (previousDistanceBetweenTouches == -1)
                        {
                            previousDistanceBetweenTouches = distanceBetweenTouches(e, 0, 1);
                        }
                        float dd = distanceBetweenTouches(e, 0, 1) - previousDistanceBetweenTouches;
                        previousDistanceBetweenTouches = distanceBetweenTouches(e, 0, 1);

                        distanceModifier += (int)(dd * SCALE_FACTOR);
                        _shape.SetBounds(20 + x, 20 + y, 300 + x + distanceModifier, 200 + y + distanceModifier);
                        this.Invalidate();
                        Console.WriteLine(string.Format("DistanceModifier: {0}, dd: {1}", distanceModifier, dd));
                    }
                    break;
                case (MotionEventActions.Down):
                case (MotionEventActions.PointerDown):
                    if (numPointers < 2) { numPointers++; }
                    Console.WriteLine("Action was DOWN. NP: {0}", numPointers);
                    return true;
                case (MotionEventActions.Up):
                case (MotionEventActions.PointerUp):
                    if (numPointers > 0) { numPointers--; }
                    Console.WriteLine("Action was UP. NP: {0}", numPointers);
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

        private bool handlePanEvent(MotionEvent e)
        {
            if (previousX == -1) { previousX = e.GetX(); }
            if (previousY == -1) { previousY = e.GetY(); }

            var dx = e.GetX() - previousX;
            var dy = e.GetY() - previousY;

            previousX = e.GetX(0);
            previousY = e.GetY(0);

            //Console.WriteLine(string.Format("({0}, {1})", dx, dy));

            x += (int)(dx * SCALE_FACTOR);
            y += (int)(dy * SCALE_FACTOR);

            _shape.SetBounds(20 + x, 20 + y, 300 + x + distanceModifier, 200 + y + distanceModifier);
            this.Invalidate();

            return true;
        }

        public float distanceBetweenTouches(MotionEvent e, int first, int second)
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