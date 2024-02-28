using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Configuration;
using System.IO;
using System.Windows.Media.Imaging;
using WpfAppAssignments;
using System.Numerics;

namespace WpfAppAssignments {
    public partial class ScribblePad : Canvas {
        Line? mLine;
        Scribble? mScribble;
        Rectangle? mRectangle;
        Ellipse? mEllipse;
        Circle? mCircle;
        PolyLines? mPolLines;
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var drawing in mDrawings) {
                drawing.DrawShape (drawingContext);
            }
        }
        public void Save (string fileName) {
            var ext = System.IO.Path.GetExtension (fileName);
            if (ext is ".txt") {
                StreamWriter sw = new (fileName, true);
                foreach (Scribble scribble in mDrawings) {
                    sw.WriteLine ($"{scribble.currentPen?.Brush},{scribble.currentPen?.Thickness}");
                    foreach (var points in scribble.mWayPoints) sw.WriteLine (points);
                }
                sw.Close ();
            } else {
                BinaryWriter bw = new (File.Open (fileName, FileMode.Append));
                foreach (Scribble scribble in mDrawings) {
                    bw.Write (scribble.currentPen.Brush.ToString ());
                    bw.Write (scribble.currentPen.Thickness);
                    foreach (var points in scribble.mWayPoints) { bw.Write (points.X); bw.Write (points.Y); }
                    bw.Write (double.NaN);
                }
                bw.Close ();
            }
        }
        public void Load (string fileName) {
            string ext = Path.GetExtension (fileName);
            if (ext is ".txt") {
                string[] points = File.ReadAllLines (fileName);
                foreach (var point in points) {
                    string[] pt = point.Split (',');
                    if (double.TryParse (pt[0], out double x) && double.TryParse (pt[1], out double y)) {
                        mScribble?.mWayPoints.Add (new Point (x, y));
                    } else {
                        var pen = new Pen ((Brush)new BrushConverter ().ConvertFrom (pt[0])!, int.Parse (pt[1]));
                        mScribble = new (pen);
                        mDrawings.Add (mScribble);
                    }
                }
            } else {
                using FileStream fs = new (fileName, FileMode.Open);
                using BinaryReader br = new (fs);
                while (br.BaseStream.Position < br.BaseStream.Length) {
                    for (; ; ) {
                        if (string.IsNullOrWhiteSpace(br.ToString())) continue;
                        var color = br.ReadString ();
                        var thick = br.ReadDouble ();
                        var pen = new Pen ((Brush)new BrushConverter ().ConvertFrom (color), thick);
                        mScribble = new (pen);
                        mDrawings.Add (mScribble);
                        var X = br.ReadDouble ();
                        if (double.IsNaN (X)) {
                            mScribble.mWayPoints.Add (new Point (double.NaN, double.NaN));
                            break;
                        }
                        var Y = br.ReadDouble ();
                        mScribble.mWayPoints.Add (new Point (X, Y));
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
        public void Erase () {
            mColor = Brushes.Black;
        }
        public void Clear () {
            mDrawings.Clear ();
            InvalidateVisual ();
        }
        public void OnClickUndo () {
            if (mDrawings.Count > 0) {
                mRedoStack.Push (mDrawings[^1]);
                mDrawings.Remove (mDrawings[^1]);
                InvalidateVisual ();
            }
        }
        public void OnClickRedo () {
            if (mRedoStack.Count > 0) {
                mDrawings.Add (mRedoStack.Pop ());
                InvalidateVisual ();
            }
        }
        protected override void OnPreviewMouseLeftButtonDown (MouseButtonEventArgs e) {
            base.OnPreviewMouseLeftButtonDown (e);
            switch (Drawing.sType) {
                case ShapeType.PLINES:
                    mIsDrawing = true;
                    mPolLines = new (new Pen (mColor, 2));
                    mPolLines.Start = mStartPoint;
                    mDrawings.Add (mPolLines);
                    //if (mPolLines.Start == mPolLines.End) mIsDrawing = false;
                    break;
            }
        }
        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                mStartPoint = e.GetPosition (this);
                switch (Drawing.sType) {
                    case ShapeType.LINE:
                        mLine = new (new Pen (mColor, 2));
                        mLine.Start = mStartPoint;
                        mDrawings.Add(mLine);
                        break;
                    case ShapeType.SCRIBBLE:
                        mScribble = new (new Pen (mColor, 2));
                        mScribble?.AddWayPoints (mStartPoint);
                        mDrawings.Add (mScribble); break;
                    case ShapeType.RECTANGLE:
                        mRectangle = new (new Pen (mColor, 2));
                        mRectangle.Start = mStartPoint;
                        mDrawings.Add (mRectangle);break;
                    case ShapeType.ELLIPSE:
                        mEllipse = new (new Pen (mColor, 2));
                        mEllipse.Start = mStartPoint;
                        mDrawings.Add (mEllipse); break;
                    case ShapeType.CIRCLE:
                        mCircle = new (new Pen (mColor, 2));
                        mCircle.Start = mStartPoint;
                        mDrawings.Add (mCircle); break;
                }
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            mEndPoint = e.GetPosition (this);
            if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                switch (Drawing.sType) {
                    case ShapeType.LINE:
                        mLine.End = mEndPoint;
                        break;
                    case ShapeType.SCRIBBLE: {
                        mScribble?.AddWayPoints (mEndPoint);
                        break;
                    }
                    case ShapeType.RECTANGLE:
                        mRectangle.End = mEndPoint;
                        break;
                    case ShapeType.ELLIPSE:
                        var X = Math.Abs(mStartPoint.X - mEndPoint.X);
                        var Y = Math.Abs(mStartPoint.Y - mEndPoint.Y);
                        mEllipse.End = new Point(X,Y);
                        break;
                    case ShapeType.CIRCLE:
                        X = Math.Abs (mStartPoint.X - mEndPoint.X);
                        mCircle.End = new Point (X, X);
                        break;
                }
                mRedoStack.Clear ();
                InvalidateVisual ();
            }
            if (mIsDrawing && e.LeftButton == MouseButtonState.Released) {
                switch (Drawing.sType) {
                    case ShapeType.PLINES:
                        mPolLines.End = mEndPoint;
                        break;
                }
            }
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released && mIsDrawing) {
                mIsDrawing = false;
                switch (Drawing.sType) {
                    case ShapeType.LINE:
                        mLine.End = mEndPoint;
                        break;
                    case ShapeType.SCRIBBLE: {
                        break;
                    }
                    case ShapeType.RECTANGLE:
                        break;
                }
            }
        }
        bool mIsDrawing = false;
        SolidColorBrush? mColor;
        Point mStartPoint, mEndPoint;
        static public List<Drawing> mDrawings = new ();
        Stack<Drawing> mRedoStack = new ();
    }
    
}