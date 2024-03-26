using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using ClassLibrary;
using static WpfAppAssignments.DocManager;
using static ClassLibrary.ShapeType;
using System.Windows;

namespace WpfAppAssignments;
#region Class ScribblePad ---------------------------------------------------------------------
public partial class ScribblePad : Canvas {
    public ScribblePad () => mNewDoc = new ();
 
    protected override void OnRender (DrawingContext drawingContext) {
        base.OnRender (drawingContext);
        foreach (var drawing in mNewDoc.mDrawings!) {
            DrawSketch.DrawShape (drawing, drawingContext);
        }
    }
    #region Methods ---------------------------------------------
    public void SaveSketch () {
        mNewDoc.Save ();
    }
    public void LoadSketch () {
        sDraw = mNewDoc.Load ();
        InvalidateVisual ();
    }
    public static Sketch CreateShape (ShapeType st) {
        sDraw = st switch {
            LINE => new Line (),
            RECTANGLE => new Rectangle (),
            CIRCLE => new Circle (),
            ELLIPSE => new Ellipse (),
            PLINES => new PolyLine (),
            _ => new Scribble ()
        };
        return sDraw;
    }
    public void Clear () {
        mNewDoc.mDrawings?.Clear ();
        sIsLoaded = false;
        InvalidateVisual ();
    }
    public void OnClickUndo () {
        if (mNewDoc.mDrawings?.Count > 0) {
            if (sIsLoaded && mNewDoc.mDrawings.Count == sLoadCnt) return;
            mNewDoc.mRedoStack?.Push (mNewDoc.mDrawings[^1]);
            mNewDoc.mDrawings.Remove (mNewDoc.mDrawings[^1]);
            InvalidateVisual ();
        }
    }
    public void OnClickRedo () {
        if (mNewDoc.mRedoStack?.Count > 0) {
            mNewDoc.mDrawings?.Add (mNewDoc.mRedoStack.Pop ());
            InvalidateVisual ();
        }
    }
    #endregion

    #region MouseEvents -----------------------------------------
    protected override void OnMouseDown (MouseButtonEventArgs e) {
        foreach (Window window in Application.Current.Windows) {
            if (window.GetType () == typeof (MainWindow)) {
                var mW = window as MainWindow;
                if (mNewDoc.Count () == 0 || (IsLoaded && mNewDoc.Count () >= sLoadCnt)) {
                    if (!mW!.Title.Contains ('*')) { mW!.Title += "*"; mNewDoc.mIsModified = true; }
                }
            }
        }
        base.OnMouseDown (e);
        var point = e.GetPosition (this);
        mStart = new Point2D (point.X, point.Y);
        if (sCurrentShape == PLINES) {
            if (e.RightButton != MouseButtonState.Pressed) {
                if (!sIsDrawing) {
                    sIsDrawing = true;
                    sDraw = new PolyLine ();
                } else {
                    if (sDraw != null) sDraw.End = mStart;
                    sDraw = new PolyLine ();
                }
                sDraw.Start = mStart;
                mNewDoc.mDrawings?.Add (sDraw);
            } else if (e.RightButton == MouseButtonState.Pressed) {
                if (sIsDrawing) mNewDoc.mDrawings?.Remove (mNewDoc.mDrawings[^1]);
                sIsDrawing = false;
                InvalidateVisual ();
            }
        }
        else if (e.ButtonState == MouseButtonState.Pressed) {
            sIsDrawing = true;
            if (sCurrentShape is SCRIBBLE) {
                mScribble = new ();
                sDraw = null;
                mScribble.AddWayPoints (mStart);
                mNewDoc.mDrawings?.Add (mScribble);
            } else {
                sDraw = CreateShape (sCurrentShape);
                sDraw.Start = mStart;
                mNewDoc.mDrawings?.Add (sDraw);
            }
        }
    }
    protected override void OnMouseMove (MouseEventArgs e) {
        base.OnMouseMove (e);
        var point = e.GetPosition (this);
        mEnd = new Point2D (point.X, point.Y);
        if (sCurrentShape == PLINES && sIsDrawing && e.RightButton != MouseButtonState.Pressed) {
            if (sDraw != null) sDraw.End = mEnd;
        } else if (sIsDrawing && e.LeftButton == MouseButtonState.Pressed) {
            if (sCurrentShape is SCRIBBLE) mScribble?.AddWayPoints (mEnd);
            else if (sDraw != null) sDraw.End = mEnd;
        }
        mNewDoc.mRedoStack?.Clear ();
        InvalidateVisual ();
    }

    protected override void OnMouseUp (MouseButtonEventArgs e) {
        if (sIsDrawing && sDraw != null) sDraw.End = mEnd;
        if (sCurrentShape != PLINES) sIsDrawing = false;
    }
    #endregion

    #region Fields ----------------------------------------------
    static Sketch? sDraw;
    Scribble? mScribble;
    DocManager mNewDoc;
    Point2D mStart, mEnd;
    public static bool sIsDrawing = false;
    public static ShapeType sCurrentShape = SCRIBBLE;
    #endregion
}
#endregion