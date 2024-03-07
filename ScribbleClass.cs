using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.IO;

namespace WpfAppAssignments {
    #region Class ScribblePad ---------------------------------------------------------------------
    public partial class ScribblePad : Canvas {
        Line? mLine;
        Scribble? mScribble;
        Rectangle? mRectangle;
        Ellipse? mEllipse;
        Circle? mCircle;
        PolyLines? mPolyLines;
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var drawing in sDrawings) {
                drawing.DrawShape (drawingContext);
            }
        }
        #region Methods ---------------------------------------------
        public void Save (string fileName) {
            BinaryWriter bw = new (File.Open (fileName, FileMode.OpenOrCreate));
            foreach (Drawing drawing in sDrawings) drawing.SaveShape (bw);
            bw.Close ();
        }
        public void Load (string fileName) {
            using FileStream fs = new (fileName, FileMode.Open);
            using (BinaryReader br = new (fs)) {
                LoadFromBinary (br);
                isLoaded = true;
                loadCnt = sDrawings.Count;
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
                    ShapeType.PLINES => new PolyLines (mPen),
                    _ => null
                };
                if (drawing == null) break;
                sDrawings.Add (drawing.LoadShape (br));
                if (br.BaseStream.Position < br.BaseStream.Length) br.ReadChar ();
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
            sDrawings.Clear ();
            InvalidateVisual ();
        }
        public void OnClickUndo () {
            if (sDrawings.Count > 0) {
                if (isLoaded && sDrawings.Count == loadCnt) return;
                mRedoStack.Push (sDrawings[^1]);
                sDrawings.Remove (sDrawings[^1]);
                InvalidateVisual ();
            }
        }
        public void OnClickRedo () {
            if (mRedoStack.Count > 0) {
                sDrawings.Add (mRedoStack.Pop ());
                InvalidateVisual ();
            }
        }
        #endregion

        #region MouseEvents -----------------------------------------
        protected override void OnMouseDown (MouseButtonEventArgs e) {
            base.OnMouseDown (e);
            mStart = e.GetPosition (this);
            Pen pen = new (mColor, 2);
            if (sCurrentShape == ShapeType.PLINES && e.RightButton != MouseButtonState.Pressed) {
                if (!mIsDrawing) {
                    mPolyLines = new (pen);
                    mIsDrawing = true;
                    mPolyLines.Start = mStart;
                }
                else {
                    if (mPolyLines != null) mPolyLines.End = mStart;
                    mPolyLines = new (pen);
                    mPolyLines.Start = mStart;
                }
                sDrawings.Add (mPolyLines);
            } else if (sCurrentShape == ShapeType.PLINES && e.RightButton == MouseButtonState.Pressed) {
                if (mIsDrawing) sDrawings.Remove (sDrawings[^1]);
                mIsDrawing = false;
                InvalidateVisual ();
            } else if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                switch (sCurrentShape) {
                    case ShapeType.LINE:
                        mLine = new (pen);
                        mLine.Start = mStart;
                        sDrawings.Add (mLine);
                        break;
                    case ShapeType.SCRIBBLE:
                        mScribble = new (pen);
                        if (mScribble != null) {
                            mScribble.AddWayPoints (mStart);
                            sDrawings.Add (mScribble);
                        }
                        break;
                    case ShapeType.RECTANGLE:
                        mRectangle = new (pen);
                        mRectangle.Start = mStart;
                        sDrawings.Add (mRectangle); break;
                    case ShapeType.ELLIPSE:
                        mEllipse = new (pen);
                        mEllipse.Start = mStart;
                        sDrawings.Add (mEllipse); break;
                    case ShapeType.CIRCLE:
                        mCircle = new (pen);
                        mCircle.Start = mStart;
                        sDrawings.Add (mCircle); break;
                }
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            mEnd = e.GetPosition (this);
            if (sCurrentShape == ShapeType.PLINES && mIsDrawing && e.RightButton != MouseButtonState.Pressed) {
                if (mPolyLines != null) mPolyLines.End = mEnd;
            }
            else if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                switch (sCurrentShape) {
                    case ShapeType.LINE:
                        mLine.End = mEnd;
                        break;
                    case ShapeType.SCRIBBLE:
                        mScribble?.AddWayPoints (mEnd);
                        break;
                    case ShapeType.RECTANGLE:
                        mRectangle.End = mEnd;
                        break;
                    case ShapeType.ELLIPSE:
                        var X = Math.Abs (mStart.X - mEnd.X);
                        var Y = Math.Abs (mStart.Y - mEnd.Y);
                        mEllipse.End = new Point (X, Y);
                        break;
                    case ShapeType.CIRCLE:
                        X = Math.Abs (mStart.X - mEnd.X);
                        mCircle.End = new Point (X, X);
                        break;
                }
            }
            mRedoStack.Clear ();
            InvalidateVisual ();
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (sCurrentShape == ShapeType.PLINES && mIsDrawing && e.RightButton != MouseButtonState.Pressed) {
                if (mPolyLines != null) mPolyLines.End = mEnd;
            }
            else if (e.LeftButton == MouseButtonState.Released && mIsDrawing) {
                mIsDrawing = false;
                switch (sCurrentShape) {
                    case ShapeType.LINE:
                        mLine.End = mEnd;
                        break;
                }
            }
        }
        #endregion

        #region Fields ----------------------------------------------
        bool mIsDrawing = false, isLoaded = false;
        int loadCnt;
        SolidColorBrush? mColor;
        Point mStart, mEnd;
        static public List<Drawing> sDrawings = new ();
        Stack<Drawing> mRedoStack = new ();
        static public ShapeType sCurrentShape = ShapeType.SCRIBBLE;
        #endregion
    }
    #endregion
}