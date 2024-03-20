using System.Windows;
using System;
using ClassLibrary;
using System.Windows.Media;
using static ClassLibrary.ShapeType;

namespace WpfAppAssignments {
    #region Class RenderShapes --------------------------------------------------------------------
    public class RenderShapes : Sketch {
        public static void DrawShape (Sketch sketch, DrawingContext dc) {
            var currentPen = new Pen (Brushes.White, 2);
            Point point (Point2D pt) => new (pt.X, pt.Y);
            var ptList = ClassLibrary.Scribble.mWayPoints;
            switch (sketch.sType) {
                case SCRIBBLE:
                    if (ptList?.Count > 1) {
                        for (int i = 0; i < ptList?.Count - 1; i++) {
                            var start = ptList[i];
                            var end = ptList[i + 1];
                            dc.DrawLine (currentPen, point(start), point(end));
                        }
                    }
                    break;
                case RECTANGLE:
                    Rect getRect = new (point(sketch.Start), point(sketch.End));
                    if (point(sketch.End) != new Point (0, 0))
                        dc.DrawRectangle (null, currentPen, getRect);
                    break;
                case LINE:
                    dc.DrawLine (currentPen, point (sketch.Start), point (sketch.End));
                    break;
                case ELLIPSE:
                    var X = Math.Abs (point (sketch.Start).X - sketch.End.X);
                    var Y = Math.Abs (sketch.Start.Y - sketch.End.Y);
                    dc.DrawEllipse (null, currentPen, point (sketch.Start), X, Y);
                    break;
                case CIRCLE:
                    X = Math.Abs (sketch.Start.X - sketch.End.X);
                    dc.DrawEllipse (null, currentPen, point (sketch.Start), X, X);
                    break;
                case PLINES:
                    dc.DrawLine (currentPen, point (sketch.Start),point(sketch.End));
                    break;
            }
        }
    }
    #endregion
}