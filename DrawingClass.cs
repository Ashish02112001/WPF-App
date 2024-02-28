using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace WpfAppAssignments {
    public class Drawing {
        static public ShapeType sType { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Pen? currentPen { get; set; }
        public virtual void DrawShape (DrawingContext drawingContext) { }
    }
    public enum ShapeType {
        SCRIBBLE,
        LINE,
        RECTANGLE,
        CIRCLE,
        ELLIPSE,
        PLINES,
    };
    public class Scribble : Drawing {
        public List<Point> mWayPoints { get; set; }
        public Scribble (Pen pen) {
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
    }
    public class Rectangle : Drawing {
        public Rectangle (Pen pen) {
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
    public class Line : Drawing {
        public Line (Pen pen) {
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawLine (currentPen, Start, End);
        }
    }
    public class Ellipse : Drawing {
        public Ellipse (Pen pen) {
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawEllipse (null, currentPen, Start, End.X, End.Y);
        }
    }
    public class Circle : Drawing {
        public Circle (Pen pen) {
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawEllipse (null, currentPen, Start, End.X, End.X);
        }
    }
    public class PolyLines : Drawing {
        public PolyLines (Pen pen) {
            currentPen = pen;
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawLine (currentPen, Start, End);
        }
    }
}
