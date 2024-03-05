﻿using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;
using System.IO;
using Brush = System.Windows.Media.Brush;
using Pen = System.Windows.Media.Pen;
using Point = System.Windows.Point;

namespace WpfAppAssignments {
    public class Drawing {
        public ShapeType sType { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public Pen? currentPen { get; set; }
        public virtual void DrawShape (DrawingContext drawingContext) { }

        public virtual void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            bw.Write (Start.X);
            bw.Write (Start.Y);
            bw.Write (End.X);
            bw.Write (End.Y);
            var brushConverter = new BrushConverter ();
            string? brushString = brushConverter.ConvertToString (currentPen.Brush);
            bw.Write (brushString);
            bw.Write (currentPen.Thickness);
            bw.Write ('\n');
        }
        public virtual Drawing LoadShape (BinaryReader br) {
            Start = new Point (br.ReadDouble (), br.ReadDouble ());
            End = new Point (br.ReadDouble (), br.ReadDouble ());
            var brushString = br.ReadString ();
            if (string.IsNullOrEmpty (brushString))
                brushString = "#FFFFFFFF";
            var thickness = br.ReadDouble ();
            currentPen = new Pen ((Brush)new BrushConverter ().ConvertFrom (brushString), thickness);
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
            bw.Write (brushString);
            bw.Write (currentPen.Thickness);
            bw.Write (mWayPoints.Count);
            foreach (var points in mWayPoints) { bw.Write (points.X); bw.Write (points.Y); }
            bw.Write ('\n');
        }
        public override Drawing LoadShape (BinaryReader br) {
            var brushString = br.ReadString ();
            if (string.IsNullOrEmpty (brushString))
                brushString = "#FFFFFFFF";
            var thickness = br.ReadDouble ();
            currentPen = new Pen ((Brush)new BrushConverter ().ConvertFrom (brushString), thickness);
            var a = br.ReadInt32 ();
            for (int i = 0; i < a; i++)
                mWayPoints.Add (new Point (br.ReadDouble (), br.ReadDouble ()));
            return this;
        }
    }
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
    public class PolyLines : Drawing {
        public List<PolyLines> mLinesColl { get; set; }
        bool mLoad = false;
        public PolyLines (Pen pen) {
            sType = ShapeType.PLINES;
            currentPen = pen;
            mLinesColl = new ();
        }
        public override void DrawShape (DrawingContext drawingContext) {
            base.DrawShape (drawingContext);
            drawingContext.DrawLine (currentPen, Start, End);
            if (mLoad) {
                foreach (var lines in mLinesColl) {
                    drawingContext.DrawLine (currentPen, lines.Start, lines.End);
                }
            }
        }
        public override void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            var brushConverter = new BrushConverter ();
            string? brushString = brushConverter.ConvertToString (currentPen?.Brush);
            bw.Write (brushString);
            bw.Write (currentPen.Thickness);
            bw.Write (mLinesColl.Count);
            foreach (var line in mLinesColl) { 
                bw.Write (line.Start.X);
                bw.Write (line.Start.Y);
                bw.Write (line.End.X);
                bw.Write (line.End.Y);
            }
            bw.Write ('\n');
        }
        public override Drawing LoadShape (BinaryReader br) {
            var brushString = br.ReadString ();
            if (string.IsNullOrEmpty (brushString))
                brushString = "#FFFFFFFF";
            var thickness = br.ReadDouble ();
            currentPen = new Pen ((Brush)new BrushConverter ().ConvertFrom (brushString), thickness);
            var cnt = br.ReadInt32 ();
            PolyLines newPolLine = new PolyLines (currentPen);
            newPolLine.Start = new Point (br.ReadDouble (), br.ReadDouble ());
            newPolLine.End = new Point (br.ReadDouble (), br.ReadDouble ());
            mLinesColl.Add (newPolLine);
            mLoad = true;
            return this;
        }
    }
}