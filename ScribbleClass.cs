using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;

namespace WpfAppAssignments {
    #region Class ScribblePad ---------------------------------------------------------------------
    public partial class ScribblePad : Canvas {
        Drawing? draw;
        Scribble? mScribble;
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var drawing in sDrawings) {
                drawing.DrawShape (drawingContext);
            }
        }
        #region Methods ---------------------------------------------
        public void Save () {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                BinaryWriter bw = new (File.Open (saveFile.FileName, FileMode.OpenOrCreate));
                foreach (Drawing drawing in sDrawings) drawing.SaveShape (bw);
                bw.Close ();
            }
        }
        public void Load () {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog () is true) {
                var fileName = openFileDialog.FileName;
                using FileStream fs = new (fileName, FileMode.Open);
                using (BinaryReader br = new (fs)) {
                    var mPen = new Pen ();
                    while (true) {
                        if (br.PeekChar () == -1) break;
                        ShapeType shapeType = (ShapeType)br.ReadInt32 ();
                        draw = CreateShape (shapeType, mPen);
                        if (draw == null) break;
                        sDrawings.Add (draw.LoadShape (br));
                        if (br.BaseStream.Position < br.BaseStream.Length) br.ReadChar ();
                    }
                    isLoaded = true;
                    loadCnt = sDrawings.Count;
                }
                InvalidateVisual ();
            }

        }
        
        Drawing CreateShape (ShapeType st, Pen mPen) {
            draw = st switch {
                ShapeType.SCRIBBLE => new Scribble (mPen),
                ShapeType.LINE => new Line (mPen),
                ShapeType.RECTANGLE => new Rectangle (mPen),
                ShapeType.CIRCLE => new Circle (mPen),
                ShapeType.ELLIPSE => new Ellipse (mPen),
                ShapeType.PLINES => new PolyLines (mPen),
                _ => null
            };
            return draw;
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
            sColor = color;
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
            Pen pen = new (sColor, 2);
            if (sCurrentShape == ShapeType.PLINES && e.RightButton != MouseButtonState.Pressed) {
                if (!mIsDrawing) {
                    mIsDrawing = true;
                    draw = new PolyLines (pen);
                } else {
                    if (draw != null) draw.End = mStart;
                    draw = new PolyLines (pen);
                }
                draw.Start = mStart;
                sDrawings.Add (draw);
            } else if (sCurrentShape == ShapeType.PLINES && e.RightButton == MouseButtonState.Pressed) {
                if (mIsDrawing) sDrawings.Remove (sDrawings[^1]);
                mIsDrawing = false;
                InvalidateVisual ();
            } else if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                switch (sCurrentShape) {
                    case ShapeType.LINE:
                        draw = new Line (pen);
                        draw.Start = mStart;
                        sDrawings.Add (draw);
                        break;
                    case ShapeType.SCRIBBLE:
                        mScribble = new (pen);
                        if (mScribble != null) {
                            mScribble.AddWayPoints (mStart);
                            sDrawings.Add (mScribble);
                        }
                        break;
                    case ShapeType.RECTANGLE:
                        draw = new Rectangle (pen);
                        draw.Start = mStart;
                        sDrawings.Add (draw);
                        break;
                    case ShapeType.ELLIPSE:
                        draw = new Ellipse (pen);
                        draw.Start = mStart;
                        sDrawings.Add (draw);
                        break;
                    case ShapeType.CIRCLE:
                        draw = new Circle (pen);
                        draw.Start = mStart;
                        sDrawings.Add (draw);
                        break;
                }
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            mEnd = e.GetPosition (this);
            if (sCurrentShape == ShapeType.PLINES && mIsDrawing && e.RightButton != MouseButtonState.Pressed) {
                if (draw != null) draw.End = mEnd;
            } else if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                if (sCurrentShape is ShapeType.SCRIBBLE) mScribble?.AddWayPoints (mEnd);
                else if (draw != null) {
                    switch (sCurrentShape) {
                        case ShapeType.LINE:
                            draw.End = mEnd;
                            break;
                        case ShapeType.RECTANGLE:
                            draw.End = mEnd;
                            break;
                        case ShapeType.ELLIPSE:
                            var X = Math.Abs (mStart.X - mEnd.X);
                            var Y = Math.Abs (mStart.Y - mEnd.Y);
                            draw.End = new Point (X, Y);
                            break;
                        case ShapeType.CIRCLE:
                            X = Math.Abs (mStart.X - mEnd.X);
                            draw.End = new Point (X, X);
                            break;
                    }
                }
            }
            mRedoStack.Clear ();
            InvalidateVisual ();
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (sCurrentShape == ShapeType.PLINES && mIsDrawing && e.RightButton != MouseButtonState.Pressed) {
                if (draw != null) draw.End = mEnd;
            } else if (e.LeftButton == MouseButtonState.Released && mIsDrawing && draw != null) {
                mIsDrawing = false;
                if (sCurrentShape == ShapeType.LINE) draw.End = mEnd;
            }
        }
        #endregion

        #region Fields ----------------------------------------------
        bool mIsDrawing = false, isLoaded = false;
        int loadCnt;
        static public SolidColorBrush? sColor = Brushes.White;
        Point mStart, mEnd;
        static public List<Drawing> sDrawings = new ();
        Stack<Drawing> mRedoStack = new ();
        static public ShapeType sCurrentShape = ShapeType.SCRIBBLE;
        #endregion
    }
    #endregion
}