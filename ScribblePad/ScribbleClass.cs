using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;
using ClassLibrary;
using static ClassLibrary.ShapeType;

namespace WpfAppAssignments;
    #region Class ScribblePad ---------------------------------------------------------------------
    public partial class ScribblePad : Canvas {
        Sketch? draw;
        Scribble? mScribble;
        protected override void OnRender (DrawingContext drawingContext) {
            base.OnRender (drawingContext);
            foreach (var drawing in sDrawings) {
                RenderShapes.DrawShape (drawing, drawingContext);
            }
        }
        #region Methods ---------------------------------------------
        public void Save () {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                BinaryWriter bw = new (File.Open (saveFile.FileName, FileMode.OpenOrCreate));
                foreach (Sketch drawing in sDrawings) drawing.SaveShape (bw);
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
                        draw = CreateShape ((ShapeType)br.ReadInt32 (), mPen);
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

        Sketch CreateShape (ShapeType st, Pen mPen) {
            draw = st switch {
                LINE => new Line (),
                RECTANGLE => new Rectangle (),
                CIRCLE => new Circle (),
                ELLIPSE => new Ellipse (),
                PLINES => new PolyLine (),
                _ => new Scribble ()
            };
            return draw;
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
            var point = e.GetPosition (this);
            mStart = new Point2D (point.X, point.Y);
            Pen pen = new (sColor, 2);
            if (sCurrentShape == PLINES) {
                if (e.RightButton != MouseButtonState.Pressed) {
                    if (!mIsDrawing) {
                        mIsDrawing = true;
                        draw = new PolyLine ();
                    } else {
                        if (draw != null) draw.End = mStart;
                        draw = new PolyLine ();
                    }
                    draw.Start = mStart;
                    sDrawings.Add (draw);
                } else if (e.RightButton == MouseButtonState.Pressed) {
                    if (mIsDrawing) sDrawings.Remove (sDrawings[^1]);
                    mIsDrawing = false;
                    InvalidateVisual ();
                }
            } else if (e.ButtonState == MouseButtonState.Pressed) {
                mIsDrawing = true;
                if (sCurrentShape is SCRIBBLE) {
                    mScribble = new ();
                    draw = null;
                    mScribble.AddWayPoints (mStart);
                    sDrawings.Add (mScribble);
                } else {
                    draw = CreateShape (sCurrentShape, pen);
                    draw.Start = mStart;
                    sDrawings.Add (draw);
                }
            }
        }
        protected override void OnMouseMove (MouseEventArgs e) {
            base.OnMouseMove (e);
            var point = e.GetPosition (this);
            mEnd = new Point2D (point.X, point.Y);
            if (sCurrentShape == PLINES && mIsDrawing && e.RightButton != MouseButtonState.Pressed) {
                if (draw != null) draw.End = mEnd;
            } else if (mIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
                if (sCurrentShape is SCRIBBLE) mScribble?.AddWayPoints (mEnd);
                else if (draw != null) draw.End = mEnd;
            }
            mRedoStack.Clear ();
            InvalidateVisual ();
        }

        protected override void OnMouseUp (MouseButtonEventArgs e) {
            if (mIsDrawing && draw != null) {
                draw.End = mEnd;
                if (sCurrentShape != PLINES) mIsDrawing = false;
            }
        }
        #endregion

        #region Fields ----------------------------------------------
        bool mIsDrawing = false, isLoaded = false;
        int loadCnt;
        static public SolidColorBrush? sColor = Brushes.White;
        Point2D mStart, mEnd;
        static public List<Sketch> sDrawings = new ();
        Stack<Sketch> mRedoStack = new ();
        static public ShapeType sCurrentShape = SCRIBBLE;
        #endregion
    }
    #endregion
