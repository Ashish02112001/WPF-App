using System.Drawing;

namespace ClassLibrary {
    public class Sketch {
        #region Properties ------------------------------------------
        public List<Point2D>? mWayPoints { get; set; }
        public ShapeType sType { get; set; }
        public Point2D Start { get; set; }
        public Point2D End { get; set; }
        #endregion

        public virtual void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            bw.Write (Start.X);
            bw.Write (Start.Y);
            bw.Write (End.X);
            bw.Write (End.Y);
            bw.Write ('\n');
        }
        public virtual Sketch LoadShape (BinaryReader br) {
            Start = new Point2D (br.ReadDouble (), br.ReadDouble ());
            End = new Point2D (br.ReadDouble (), br.ReadDouble ());
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
    
    public struct Point2D {
        public Point2D (double x, double y) {
            X = x; Y = y;
        }
        public double X { get; set;}
        public double Y { get; set;}
    }
}
