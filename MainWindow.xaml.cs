using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace WpfAppAssignments {
    public partial class MainWindow : Window {
        public MainWindow () {
            InitializeComponent ();
        }

        private void Button_Click_Erase (object sender, RoutedEventArgs e) {
            //mScribble.mPen = new (Brushes.Black, mScribble.mPen.Thickness);
        }

        private void Button_Click_Clear (object sender, RoutedEventArgs e) {
            mScribble.Children.Clear ();
        }
        private void mSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            mScribble.ColorChange (mSelect.SelectedIndex);
        }

        private void Button_Click_Save (object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                var ext = System.IO.Path.GetExtension (saveFile.FileName);
                if (ext is ".txt") {
                    StreamWriter sw = new (saveFile.FileName, true);
                    foreach (var points in ScribblePad.mWayPoints) sw.WriteLine (points);
                    sw.Close ();
                } else {
                    BinaryWriter bw = new BinaryWriter (File.Open (saveFile.FileName, FileMode.Append));
                    foreach (var points in ScribblePad.mWayPoints) {
                        bw.Write (points.X); bw.Write (points.Y); 
                    }
                    bw.Write (double.NaN);
                    bw.Close ();
                }
            }
        }

        private void Button_Click_Load (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog () is true) {
                var fileName = openFileDialog.FileName;
                mScribble.Load (fileName);
                //mScribble.Load (fileName);
            }
        }
    }
}