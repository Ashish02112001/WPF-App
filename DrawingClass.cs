using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Brush = System.Windows.Media.Brush;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace WpfAppAssignments {
    #region Class Drawing -------------------------------------------------------------------------
    public class Drawing {
        #region Properties ------------------------------------------
        public ShapeType sType { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Pen? currentPen { get; set; }
        #endregion
        public virtual void DrawShape (DrawingContext dc) { }

        public virtual void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            bw.Write (Start.X);
            bw.Write (Start.Y);
            bw.Write (End.X);
            bw.Write (End.Y);
            var brushConverter = new BrushConverter ();
            string? brushString = brushConverter.ConvertToString (currentPen?.Brush);
            bw.Write (brushString ?? "#FFFFFFFF");
            bw.Write (currentPen != null ? currentPen.Thickness : 2);
            bw.Write ('\n');
        }
        public virtual Drawing LoadShape (BinaryReader br) {
            Start = new Point (br.ReadDouble (), br.ReadDouble ());
            End = new Point (br.ReadDouble (), br.ReadDouble ());
            var brushString = br.ReadString ();
            if (string.IsNullOrEmpty (brushString))
                brushString = "#FFFFFFFF";
            var thickness = br.ReadDouble ();
            currentPen = new Pen ((Brush)new BrushConverter ().ConvertFrom (brushString)!, thickness);
            return this;
        }
    }
    public enum ShapeType {
        SCRIBBLE,
        LINE,
        RECTANGLE,
        CIRCLE,
        ELLIPSE,
        PLINES,
    };
    #endregion

    #region Class Scribble ------------------------------------------------------------------------
    public class Scribble : Drawing {
        public List<Point> mWayPoints { get; set; }
        public Scribble (Pen pen) {
            sType = ShapeType.SCRIBBLE;
            currentPen = pen;
            mWayPoints = new ();
        }
        public void AddWayPoints (Point pt) => mWayPoints.Add (pt);
        public override void DrawShape (DrawingContext dc) {
            if (mWayPoints.Count > 1) {
                for (int i = 0; i < mWayPoints.Count - 1; i++) {
                    var start = mWayPoints[i];
                    var end = mWayPoints[i + 1];
                    dc.DrawLine (currentPen, start, end);
                }
            }
        }
        public override void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            var brushConverter = new BrushConverter ();
            string? brushString = brushConverter.ConvertToString (currentPen?.Brush);
            bw.Write (brushString ?? "#FFFFFFFF");
            bw.Write (currentPen != null ? currentPen.Thickness : 2);
            bw.Write (mWayPoints.Count);
            foreach (var points in mWayPoints) { bw.Write (points.X); bw.Write (points.Y); }
            bw.Write ('\n');
        }
        public override Drawing LoadShape (BinaryReader br) {
            var brushString = br.ReadString ();
            if (string.IsNullOrEmpty (brushString))
                brushString = "#FFFFFFFF";
            var thickness = br.ReadDouble ();
            currentPen = new Pen ((Brush)new BrushConverter ().ConvertFrom (brushString)!, thickness);
            var a = br.ReadInt32 ();
            for (int i = 0; i < a; i++)
                mWayPoints.Add (new Point (br.ReadDouble (), br.ReadDouble ()));
            return this;
        }
    }
    #endregion

    #region Class Rectangle -----------------------------------------------------------------------
    public class Rectangle : Drawing {
        public Rectangle (Pen pen) {
            sType = ShapeType.RECTANGLE;
            currentPen = pen;
        }
        public double Width { get; set; }
        public double Height { get; set; }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            Rect getRect = new (Start, End);
            drawingContext.DrawRectangle (null, currentPen, getRect);
        }
    }
    #endregion

    #region Class Line ----------------------------------------------------------------------------
    public class Line : Drawing {
        public Line (Pen pen) {
            sType = ShapeType.LINE;
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawLine (currentPen, Start, End);
        }
    }
    #endregion

    #region Class Ellipse -------------------------------------------------------------------------
    public class Ellipse : Drawing {
        public Ellipse (Pen pen) {
            sType = ShapeType.ELLIPSE;
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawEllipse (null, currentPen, Start, End.X, End.Y);
        }
    }
    #endregion

    #region Class Circle --------------------------------------------------------------------------
    public class Circle : Drawing {
        public Circle (Pen pen) {
            sType = ShapeType.CIRCLE;
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawEllipse (null, currentPen, Start, End.X, End.X);
        }
    }
    #endregion

    #region Class PolyLines -----------------------------------------------------------------------
    public class PolyLines : Drawing {
        public PolyLines (Pen pen) {
            sType = ShapeType.PLINES;
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawLine (currentPen, Start, End);
        }
    }
    #endregion
}