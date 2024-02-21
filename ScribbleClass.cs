using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.IO;

namespace WpfAppAssignments {
    public partial class ScribblePad : Canvas {
        bool isDrawing = false;
        Point mStartPoint, mEndPoint;
        static public List<Point> mWayPoints = new ();
        public Pen mPen = new (Brushes.White, 2);
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            if (mWayPoints.Count > 1) {
                for (int i = 0; i < mWayPoints.Count - 1; i++) {
                    if (mWayPoints[i] == mX) continue;
                    var start = mWayPoints[i];
                    var end = mWayPoints[i + 1];
                    drawingContext.DrawLine (mPen, start, end);
                }
            }
        }
        public void Load (string fileName) {
            string[] points = File.ReadAllLines (fileName);
            foreach (var point in points) {
                string[] pt = point.Split (',');
                if (double.TryParse (pt[0], out double x) && double.TryParse (pt[1], out double y)) {
                    mWayPoints.Add (new Point (x, y));
                }
            }
            InvalidateVisual ();
        }

        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            if (e.ButtonState == MouseButtonState.Pressed) {
                isDrawing = true;
                mStartPoint = e.GetPosition (this);
                mWayPoints.Add (mStartPoint);
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed) {
                mEndPoint = e.GetPosition (this);
                mWayPoints.Add (mEndPoint);
                InvalidateVisual ();
            }
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released) {
                isDrawing = false;
            }
            mWayPoints.Add (mX);
        }
        Point mX = new (double.NaN, double.NaN);
    }
}