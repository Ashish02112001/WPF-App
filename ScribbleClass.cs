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

namespace WpfAppAssignments {
    public partial class ScribblePad : Canvas {
        bool mIsDrawing = false;
        SolidColorBrush mColor;
        Point mStartPoint, mEndPoint;
        static public List<Scribble> mScribbles = new ();
        Stack<Scribble> mUndoStack = new (), mRedoStack = new ();
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var scribble in mScribbles) {
                if (scribble.mWayPoints.Count > 1) {
                    for (int i = 0; i < scribble.mWayPoints.Count - 1; i++) {
                        if (scribble.mWayPoints[i] == mX) continue;
                        var start = scribble.mWayPoints[i];
                        var end = scribble.mWayPoints[i + 1];
                        drawingContext.DrawLine (scribble.mPen, start, end);
                    }
                }
            }
        }
        public void Save (string fileName) {
            var ext = System.IO.Path.GetExtension (fileName);
            if (ext is ".txt") {
                StreamWriter sw = new (fileName, true);
                foreach (var scribble in mScribbles) {
                    sw.WriteLine ($"{scribble.mPen.Brush},{scribble.mPen.Thickness}");
                    foreach (var points in scribble.mWayPoints) sw.WriteLine (points);
                }
                sw.Close ();
            } else {
                BinaryWriter bw = new (File.Open (fileName, FileMode.Append));
                foreach (var scribble in mScribbles) {
                    bw.Write (scribble.mPen.Brush.ToString());
                    bw.Write (scribble.mPen.Thickness);
                    foreach (var points in scribble.mWayPoints) {bw.Write (points.X); bw.Write (points.Y); }
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
                        mScribbles[^1].mWayPoints.Add (new Point (x, y));
                    }
                    else {
                        var pen = new Pen ((Brush)new BrushConverter ().ConvertFrom (pt[0])!, int.Parse (pt[1]));
                        Scribble newScribble = new (pen);
                        mScribbles.Add (newScribble);
                    }
                }
            } else {
                using FileStream fs = new (fileName, FileMode.Open);
                using BinaryReader br = new (fs);
                while (br.BaseStream.Position < br.BaseStream.Length) {
                    for (; ; ) {
                        string color = br.ReadString ();
                        int thick = br.ReadInt32 ();
                        var pen = new Pen ((Brush)new BrushConverter ().ConvertFrom (color), thick);
                        Scribble newScribble = new (pen);
                        mScribbles.Add (newScribble);
                        var X = br.ReadDouble ();
                        if (double.IsNaN (X)) {
                            mScribbles[^1].mWayPoints.Add (new Point (double.NaN, double.NaN)); 
                            break;
                        }
                        var Y = br.ReadDouble ();
                        mScribbles[^1].mWayPoints.Add (new Point (X, Y));
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
        public void OnClickUndo () {
            if (mScribbles.Count > 0) {
                mRedoStack.Push (mScribbles[^1]);
                mScribbles.Remove (mScribbles[^1]);
                InvalidateVisual();
            }
        }
        public void OnClickRedo () {
            if (mRedoStack.Count > 0) {
                mScribbles.Add (mRedoStack.Pop ());
                InvalidateVisual() ;
            }
        }
        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                Scribble newScrib = new (new Pen(mColor,2));
                mStartPoint = e.GetPosition (this);
                newScrib.mWayPoints.Add (mStartPoint);
                mScribbles.Add (newScrib);
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                mEndPoint = e.GetPosition (this);
                mScribbles[^1].mWayPoints.Add (mEndPoint);
                mRedoStack.Clear ();
                InvalidateVisual ();
            }
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released) {
                mIsDrawing = false;
                mUndoStack.Push (mScribbles[^1]);
            }
            mScribbles[^1].mWayPoints.Add (mX);
        }
        Point mX = new (double.NaN, double.NaN);
    }
    public class Scribble {
        public Pen mPen { get; set; }
        public List<Point> mWayPoints { get; set; }
        public Scribble (Pen pen) {
            mPen = pen;
            mWayPoints = new ();
        }
    }
}