using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace WpfAppAssignments {
    public partial class ScribblePad : Canvas {
        Line? mLine;
        Scribble? mScribble;
        Rectangle? mRectangle;
        Ellipse? mEllipse;
        Circle? mCircle;
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var drawing in mDrawings) {
                drawing.DrawShape (drawingContext);
            }
        }
        public void Save (string fileName) {
            BinaryWriter bw = new (File.Open (fileName, FileMode.OpenOrCreate));
            foreach (Drawing drawing in mDrawings) drawing.SaveShape (bw);
            bw.Close ();
        }
        public void Load (string fileName) {
            using FileStream fs = new (fileName, FileMode.Open);
            using (BinaryReader br = new (fs)) {
                LoadFromBinary (br);
            }
            InvalidateVisual ();
        }

        void LoadFromBinary (BinaryReader br) {
            Drawing? drawing;
            var mPen = new Pen ();
            while (true) {
                if (br.PeekChar () == -1) break;
                ShapeType shapeType = (ShapeType)br.ReadInt32 ();
                drawing = shapeType switch {
                    ShapeType.SCRIBBLE => new Scribble (mPen),
                    ShapeType.LINE => new Line (mPen),
                    ShapeType.RECTANGLE => new Rectangle (mPen),
                    ShapeType.CIRCLE => new Circle (mPen),
                    ShapeType.ELLIPSE => new Ellipse (mPen),
                };

                if (drawing == null) break;
                mDrawings.Add (drawing.LoadShape (br));

                if (br.BaseStream.Position < br.BaseStream.Length) br.ReadChar() ;
            }
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

        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                mStartPoint = e.GetPosition (this);
                switch (mCurrentShape) {
                    case ShapeType.LINE:
                        mLine = new (new Pen (mColor, 2));
                        mLine.Start = mStartPoint;
                        mDrawings.Add (mLine);
                        break;
                    case ShapeType.SCRIBBLE:
                        mScribble = new (new Pen (mColor, 2));
                        mScribble?.AddWayPoints (mStartPoint);
                        mDrawings.Add (mScribble); break;
                    case ShapeType.RECTANGLE:
                        mRectangle = new (new Pen (mColor, 2));
                        mRectangle.Start = mStartPoint;
                        mDrawings.Add (mRectangle); break;
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
                switch (mCurrentShape) {
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
                        var X = Math.Abs (mStartPoint.X - mEndPoint.X);
                        var Y = Math.Abs (mStartPoint.Y - mEndPoint.Y);
                        mEllipse.End = new Point (X, Y);
                        break;
                    case ShapeType.CIRCLE:
                        X = Math.Abs (mStartPoint.X - mEndPoint.X);
                        mCircle.End = new Point (X, X);
                        break;
                }
                mRedoStack.Clear ();
                InvalidateVisual ();
            }
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Released && mIsDrawing) {
                mIsDrawing = false;
                switch (mCurrentShape) {
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
        static public ShapeType mCurrentShape = ShapeType.SCRIBBLE;
    }
}