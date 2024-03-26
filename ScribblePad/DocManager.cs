using ClassLibrary;
using Microsoft.Win32;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;

namespace WpfAppAssignments {
    public class DocManager {
        public DocManager() {
            mRedoStack = new ();
            mDrawings = new ();
        }
        public static string? Name { get; set; } = "Untitled";

        public void Save () {
            SaveFileDialog saveFile = new () {
                Filter = "Binary files (*.bin)|*.bin"
            };
            if (saveFile.ShowDialog () == true) {
                BinaryWriter bw = new (File.Open (saveFile.FileName, FileMode.OpenOrCreate));
                foreach (Sketch drawing in mDrawings!) drawing.SaveShape (bw);
                bw.Close ();
                Name = saveFile.SafeFileName;
            }
            mIsModified = false;
        }
        public int Count () => mDrawings!.Count;
        public Sketch Load () {
            OpenFileDialog openFileDialog = new () {
                Filter = "Binary files (*.bin)|*.bin"
            };
            Sketch sketch = new ();
            if (openFileDialog.ShowDialog () is true) {
                var filePath = openFileDialog.FileName;
                using FileStream fs = new (filePath, FileMode.Open);
                using (BinaryReader br = new (fs)) {
                    var mPen = new Pen ();
                    while (true) {
                        if (br.PeekChar () == -1) break;
                        sketch = ScribblePad.CreateShape ((ShapeType)br.ReadInt32 ());
                        if (sketch == null) break;
                        mDrawings?.Add (sketch.LoadShape (br));
                        if (br.BaseStream.Position < br.BaseStream.Length) br.ReadChar ();
                    }
                    sIsLoaded = true;
                    sLoadCnt = mDrawings!.Count;
                }
                Name = openFileDialog.SafeFileName;
            }
            return sketch!;
        }
        public static bool sIsLoaded;
        public static int sLoadCnt;
        public bool mIsModified;
        public Stack<Sketch>? mRedoStack;
        public List<Sketch>? mDrawings;
    }
}