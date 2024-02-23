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
        bool mIsDrawing = false;
        SolidColorBrush mColor;
        Point mStartPoint, mEndPoint;
        static public List<Point> mWayPoints = new ();
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            if (mWayPoints.Count > 1) {
                for (int i = 0; i < mWayPoints.Count - 1; i++) {
                    if (mWayPoints[i] == mX) continue;
                    var start = mWayPoints[i];
                    var end = mWayPoints[i + 1];
                    drawingContext.DrawLine (new Pen (mColor, 2), start, end);
                }
            }
        }
        public void Load (string fileName) {
            string ext = Path.GetExtension (fileName);
            if (ext is ".txt") {
                string[] points = File.ReadAllLines (fileName);
                foreach (var point in points) {
                    string[] pt = point.Split (',');
                    if (double.TryParse (pt[0], out double x) && double.TryParse (pt[1], out double y)) {
                        mWayPoints.Add (new Point (x, y));
                    }
                }
            } else {
                using FileStream fs = new (fileName, FileMode.Open);
                using BinaryReader br = new (fs);
                while (br.BaseStream.Position < br.BaseStream.Length) {
                    for (; ; ) {
                        var X = br.ReadDouble ();
                        if (double.IsNaN (X)) { mWayPoints.Add (new Point (double.NaN, double.NaN)); break; }
                        var Y = br.ReadDouble ();
                        mWayPoints.Add (new Point (X, Y));
                    }
                }
            }
            InvalidateVisual ();
        }
        public void ColorChange (int choice) {
            SolidColorBrush color = Brushes.White;
            switch (choice) {
                case 0: color = Brushes.White; break;
                case 1: color = Brushes.Green; break;
                case 2: color = Brushes.Orange; break;
                case 3: color = Brushes.Red; break;
                case 4: color = Brushes.Yellow; break;
            }
            mColor = color;
        }
        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                mStartPoint = e.GetPosition (this);
                mWayPoints.Add (mStartPoint);
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                mEndPoint = e.GetPosition (this);
                mWayPoints.Add (mEndPoint);
                InvalidateVisual ();
            }
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released) {
                mIsDrawing = false;
            }
            mWayPoints.Add (mX);
        }
        Point mX = new (double.NaN, double.NaN);
    }
}