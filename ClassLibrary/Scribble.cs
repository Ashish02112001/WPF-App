using static ClassLibrary.ShapeType;

namespace ClassLibrary {
    public class Scribble : Sketch {
        public static List<Point2D>? mWayPoints { get; set; }
        public Scribble () {
            sType = SCRIBBLE;
            mWayPoints = new ();
        }
        public void AddWayPoints (Point2D pt) => mWayPoints.Add (pt);

        public override void SaveShape (BinaryWriter bw) {
            bw.Write ((int)sType);
            bw.Write (mWayPoints.Count);
            foreach (var points in mWayPoints) { bw.Write (points.X); bw.Write (points.Y); }
            bw.Write ('\n');
        }
        public override Sketch LoadShape (BinaryReader br) {
            var a = br.ReadInt32 ();
            for (int i = 0; i < a; i++)
                AddWayPoints (new Point2D (br.ReadDouble (), br.ReadDouble ()));
            return this;
        }
    }
}